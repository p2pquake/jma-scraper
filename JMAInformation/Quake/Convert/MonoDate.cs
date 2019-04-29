using System;
using System.Text.RegularExpressions;

namespace P2PQuake.JMAInformation.Quake.Convert
{
    /// <summary>
    /// ランタイムに依存しない日時のパーサクラス
    /// </summary>
    public class MonoDate
    {
        /// <summary>
        /// <p>日時をパースします．</p>
        /// <ul>
        /// <li>和暦は「平成」「令和」のみ対応です．未対応の元号が入力された場合，現在日時により今年または昨年として補完します．</li>
        /// <li>「元年」記法にも対応しています．</li>
        /// </ul>
        /// </summary>
        /// <param name="input">年月日時分（西暦または和暦）</param>
        public static DateTime Parse(string input)
        {
            string s = Regex.Replace(input, "[ 　]+", "");

            // ふつうの西暦表記
            Match seirekiMatch = Regex.Match(s , @"^(?<year>\d+)年(?<month>\d+)月(?<day>\d+)日(?<hour>\d+)時(?<min>\d+)分$");
            if (seirekiMatch.Success)
            {
                //Console.WriteLine("西暦マッチ");
                return DateTime.ParseExact(s, "yyyy年M月d日H時m分", null);
            }

            // 和暦表記
            Match warekiMatch = Regex.Match(s , @"^(?<nengou>\S+?)(?<year>\d+|元)(?<postfix>年\d+月\d+日\d+時\d+分)$");
            if (warekiMatch.Success)
            {
                //Console.WriteLine("和暦マッチ");
                int offsetYear = CalcNengoOffset(warekiMatch.Groups["nengou"].Value);
                if (offsetYear > 0)
                {
                    // 和暦が認識できた場合
                    string date = (offsetYear + ParseYear(warekiMatch.Groups["year"].Value)).ToString() +
                                  warekiMatch.Groups["postfix"];
                    return DateTime.ParseExact(date, "yyyy年M月d日H時m分", null);
                }
                else
                {
                    // 和暦が認識できなかった場合 (推定する)
                    DateTime baseTime = DateTime.Now;
                    int todayDays = baseTime.DayOfYear;

                    Match mdMatch = Regex.Match(s, @"\d+月\d+日");
                    int matchDays = DateTime.ParseExact(mdMatch.Value, "M月d日", null).DayOfYear;

                    int year;
                    if (todayDays >= matchDays)
                    {
                        // 現在日 >= 解析対象日: 解析対象は今年である可能性が高い
                        year = baseTime.Year;
                    }
                    else
                    {
                        // 現在日 <  解析対象日: 解析対象は昨年である可能性が高い
                        year = baseTime.Year - 1;
                    }

                    string date = year.ToString() + warekiMatch.Groups["postfix"];
                    return DateTime.ParseExact(date, "yyyy年M月d日H時m分", null);
                }
            }

            throw new FormatException("与えられた日時は解析できない形式です。");
        }

        private static int CalcNengoOffset(string nengo)
        {
            if (nengo == "平成")
                return 1988;
            if (nengo == "令和")
                return 2018;

            return -1;
        }

        private static int ParseYear(string year)
        {
            if (year == "元")
                return 1;

            return int.Parse(year);
        }
    }
}

