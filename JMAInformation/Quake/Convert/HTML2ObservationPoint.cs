using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;

namespace P2PQuake.JMAInformation.Quake.Convert
{
    /// <summary>
    /// HTMLソースを解析し，震度観測点をIList<ObservationPoint>として返すクラス
    /// </summary>
    class HTML2ObservationPoint
    {

        public HTML2ObservationPoint()
        {
        }
  
        /// <summary>
        /// HTMLソースを解析し，震度観測点をIList<ObservationPoint>で返します．
        /// </summary>
        /// <param name='html'>
        /// HTMLソース
        /// </param>
        /// <param name='type'>
        /// 地震情報種類（解析に必要）
        /// </param>
        public IList<ObservationPoint> Do(string html, IssueType type)
        {
            IList<ObservationPoint> points = null;
            string[] array = html2StrArray(html);

            if (array != null)
                array = deleteNeedless(array);
            if (array != null)
                points = strArray2List(array, type);

            return points;
        }

        private string[] html2StrArray(string html)
        {
            // <pre>タイプか，<table>タイプか．それが問題
            string[] strArray = null;

            strArray = pre2StrArray(html);
            if (strArray == null)
                strArray = table2StrArray(html);

            return strArray;
        }

        private string[] pre2StrArray(string html)
        {
            string[] strArray = null;

            Match match = Regex.Match(html, @"<pre.*?>(.+?)</pre>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string body = match.Groups[1].Value;
                body = Regex.Replace(body, @"[\\r\\n]+[ 　	]+", " +");
                body = Regex.Replace(body, @"[¥\r¥\n 　	]+", " *", RegexOptions.Multiline);
                body = Regex.Replace(body, "[ 　	]+", " ");
                body = Regex.Replace(body, "^ ", "");

                strArray = body.Split(' ');
            }

            return strArray;
        }

        private string[] table2StrArray(string html)
        {
            string[] strArray = null;

            Match match = Regex.Match(html, @"<table.*?>(.+?>震度[１２３４５６７].+?)</table>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                // 削っていく
                string body = match.Groups[1].Value;

                while (true)
                {
                    Match shortMatch = Regex.Match(body, @"<table.*?>(.*震度[１２３４５６７].*)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                    if (shortMatch.Success)
                        body = shortMatch.Groups[1].Value;
                    else
                        break;
                }

                body = Regex.Replace(body, @"<tr.*?>[\¥r\¥n 　	]*<td.*?>[\¥r\¥n 　	]*", " *");
                body = Regex.Replace(body, "<.+?>", " ");
                body = Regex.Replace(body, @"[¥\r¥\n 　	]+", " ", RegexOptions.Multiline);
                body = Regex.Replace(body, "^ ", "");

                strArray = body.Split(' ');
            }

            return strArray;
        }

        private string[] deleteNeedless(string[] array)
        {
            string cat = string.Join("", array);            

            // 不要な情報が加えられているかチェック
            // (〜〜〜"発表"の文字があるかどうかを調べる）
            if (!cat.Contains("発表"))
                return array;

            // 開始位置サーチ
            //  "［震度X以上が観測された" or "震度X" or "〜〜都道府県"
            int startIndex = -1;
            for (int i = 0; i < array.Length; i++)
            {
                if (
                    Regex.IsMatch(array[i], "^[+*]［震度[１２３４５６７]") ||
                    Regex.IsMatch(array[i], "^[+*]震度[１２３４５６７][弱強]?$") ||
                    Regex.IsMatch(array[i], "^[+*].+[都道府県]$")
                    )
                {
                    startIndex = i;
                    break;
                }
            }

            if (startIndex < 0)
                return null;

            // 終了位置サーチ
            //  "津波" という文字を含む行まで．
            int endIndex = array.Length;
            for (int i = startIndex + 1; i < array.Length; i++)
            {
                if (array[i].Contains("津波"))
                {
                    endIndex = i;
                    break;
                }
            }

            int length = endIndex - startIndex;

            // 検索した範囲をコピーして返す
            string[] returnArray = new string[length];
            Array.Copy(array, startIndex, returnArray, 0, length);

            return returnArray;
        }

        private IList<ObservationPoint> strArray2List(string[] array, IssueType type)
        {
            IList<ObservationPoint> list = new List<ObservationPoint>(array.Length);

            // (IssueType.ScaleAndDestination) 地域かどうか
            bool isAreaMode = (type == IssueType.ScalePrompt);
            // 都道府県
            string prefecture = "";
            // 震度(10倍, "弱"は-5, 推定は+1)
            int scale = -1;

            foreach (string e in array)
            {
                string element = e.Replace("+", "").Replace("*", "");
                
                // 震度が素直に記されている場合
                if (Regex.IsMatch(element, "^震度[１２３４５６７][弱強]?$")) {
                    Match match = Regex.Match(element, "^震度([１２３４５６７])([弱強]?)$");
                    scale = scaleToInt(match.Groups[1].Value, match.Groups[2].Value);
                }
                // 都道府県が素直に記されている場合
                else if (Regex.IsMatch(element, "^.+[都道府県]$"))
                {
                    prefecture = element;
                }
                // よくわからんが5弱以上と推定されるらしい場合
                else if (Regex.IsMatch(element, "^.+５弱以上と.+$"))
                {
                    scale = 50 - 5 + 1;
                }
                // よくわからんがどうでもよさそうな場合
                else if (Regex.IsMatch(element, "^[［（【〈《〔｛].+"))
                {
                    // 地域か市町村かは結構重要だぞ．
                    if (Regex.IsMatch(element, "地域"))
                        isAreaMode = true;
                    else if (Regex.IsMatch(element, "市区?町村"))
                        isAreaMode = false;
                    else
                        isAreaMode = false;
                }
                // 空っぽじゃん
                else if (element == "")
                {
                    
                }
                // 結局観測点っぽい
                else if (scale > 0)
                {
                    ObservationPoint point;
                    point.isArea = isAreaMode;
                    point.seismicScale = scale;
                    point.address = element.Replace("＊", "");

                    point.prefecture = prefecture;
                    // "大阪府南部" → "大阪府" をprefectureにする，など
                    if (isAreaMode)
                    {
                        Match match = Regex.Match(element, @"^([^市区町村]{2}[都道府県]|[^市区町村]{3}県)");
                        if (match.Success)
                            point.prefecture = match.Groups[1].Value;
                    }

                    if (isAreaMode)
                    {
                        point.pointType = ObservationPointType.Unknown;
                    }
                    else
                    {
                        point.pointType = (element.Contains("＊") ? ObservationPointType.LocalOrNIED : ObservationPointType.JMA);
                    }

                    list.Add(point);
                }
            }

            return list;
        }

        private int scaleToInt(string level, string level2)
        {
            char halfLevel = (char)('0' + (level[0] - '０'));
            int scale = int.Parse(halfLevel.ToString()) * 10;

            if (level2 == "弱")
                scale -= 5;

            return scale;
        }

        private string fullNum2Half(string includeString)
        {
            includeString = Regex.Replace(includeString, "[０-９]", new MatchEvaluator(num2Half));
            includeString = includeString.Replace('．', '.');
            return includeString;
        }

        private string num2Half(Match match)
        {
            char halfNum = (char)('0' + (match.Value[0] - '０'));
            return halfNum.ToString();
        }
    }
}
