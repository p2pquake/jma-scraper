using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using System.Text.RegularExpressions;
using P2PQuake.JMAInformation.Common.Convert;

namespace P2PQuake.JMAInformation.Quake.Convert
{
    /// <summary>
    /// HTMLソースを解析し，JMAQuakeインスタンスを生成するクラス
    /// </summary>
    class HTML2JMAQuake
    {
        public HTML2JMAQuake()
        {

        }

        /// <summary>
        /// HTMLソースを解析し，解析結果をJMAQuakeインスタンスとして返します．
        /// </summary>
        /// <param name='html'>
        /// HTMLソース
        /// </param>
        public JMAQuake Do(string html)
        {
            JMAQuake jmaQuake = new JMAQuake();

            init(html);
            jmaQuake.issueType = getIssueType();
            jmaQuake.issueTime = getIssueTime();
            jmaQuake.issueOf = getIssueOf();

            if (jmaQuake.issueType != IssueType.Other && jmaQuake.issueType != IssueType.Unknown)
            {
                jmaQuake.occuredTime = getOccuredTime();
                jmaQuake.occuredPlace = getOccuredPlace();
                jmaQuake.occuredLatitude = getOccuredLatitude();
                jmaQuake.occuredLongitude = getOccuredLongitude();
                jmaQuake.occuredDepth = getOccuredDepth();
                jmaQuake.occuredMagnitude = getOccuredMagnitude();
                jmaQuake.occuredScale = getOccuredScale();
                jmaQuake.domesticTsunamiType = getDomesticTsunamiType();
                jmaQuake.foreignTsunamiType = getForeignTsunamiType();
                jmaQuake.correctType = getCorrectType();
                jmaQuake.observationPoints = getObservationPoints();
            }

            return jmaQuake;
        }

        private IssueType getIssueType()
        {
            string html = getHtml(true, false, false, true);
            Dictionary<string, IssueType> issueTable = new Dictionary<string, IssueType>(8);
            IssueType issue = IssueType.Unknown;

            issueTable.Add("震度.{0,2}速報", IssueType.ScalePrompt);
            issueTable.Add("震源.{0,6}情報", IssueType.Destination);
            issueTable.Add("震源.{0,6}震度.{0,6}情報", IssueType.ScaleAndDestination);
            issueTable.Add("各地.{0,4}震度.{0,6}情報", IssueType.DetailScale);
            issueTable.Add("遠地.{0,4}地震.{0,6}情報", IssueType.Foreign);
            issueTable.Add("その他.{0,4}情報", IssueType.Other);

            foreach (string issueString in issueTable.Keys)
            {
                string pattern = "(【|<h\\d>.{0,12})" + issueString + "(】|.{0,4}<\\/h\\d>)";
                Match match = Regex.Match(html, pattern);

                if (match.Success)
                    issue = issueTable[issueString];
                else
                {
                    match = Regex.Match(html, issueString);
                    if (match.Success && issue == IssueType.Unknown)
                        issue = issueTable[issueString];
                }
            }

            return issue;
        }

        private string getIssueOf()
        {
            string html = getHtml(true, false, true, false);
            string issueOf = "";

            string[] patterns = new string[] {
                "[ 　]+([^ 　]{3,8}?)発表",
                "分[ 　]+([^ 　]{2,6}?(管区気象台|庁|管区))" 
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(html, pattern);
                if (match.Success)
                {
                    issueOf = match.Groups[1].Value;
                    break;
                }
            }

            return issueOf;
        }

        private DateTime getIssueTime()
        {
            string html = getHtml(true, false, false, false, true);
            DateTime issueTime = DateTime.MinValue;

            Match match = Regex.Match(html, @"(..)\s*(\d+|元)年\s*(\d+)月\s*(\d+)日\s*(\d+)時\s*(\d+)分");
            if (match.Success)
                issueTime = MonoDate.Parse(match.Value); //, new CultureInfo("ja-JP", true));

            return issueTime;
        }

        private DateTime getOccuredTime()
        {
            string html = getHtml(true, false, false, false, true);
            DateTime occuredTime = DateTime.MinValue;

            Match match = Regex.Match(html, @"(\d+)日\s*(\d+)時\s*(\d+)分(ころ|ごろ|頃)");
            if (match.Success)
            {
                DateTime now = getProtocolTime(0);
                int day = int.Parse(match.Groups[1].Value);

                if (day > now.Day)
                    now = now.AddMonths(-1);

                occuredTime = MonoDate.Parse(
                    now.Year.ToString() + "年" + now.Month.ToString() + "月" + match.Groups[1].Value + "日" +
                    match.Groups[2].Value + "時" + match.Groups[3].Value + "分"
                );
            }

            return occuredTime;
        }

        private string getOccuredPlace()
        {
            string html = getHtml(true, true, true, true, true);
            string occuredPlace = "";

            string[] patterns = new string[] {
                "詳しい震源.{0,4}は(.{2,36}?)です。",
                "震源地は、?(.{2,24}?)（"
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(html, pattern);
                if (match.Success)
                {
                    occuredPlace = match.Groups[1].Value;
                    break;
                }
            }

            return occuredPlace;
        }

        private double getOccuredLatitude()
        {
            string html = getHtml(true, true, true, true, true);
            double occuredLatitude = -200;

            Match match = Regex.Match(html, @"([南北])緯([0-9.]+)度");
            if (match.Success)
            {
                occuredLatitude = double.Parse(match.Groups[2].Value);
                if (match.Groups[1].Value == "南")
                    occuredLatitude *= -1;
            }

            return occuredLatitude;
        }

        private double getOccuredLongitude()
        {
            string html = getHtml(true, true, true, true, true);
            double occuredLongitude = -200;

            Match match = Regex.Match(html, @"([東西])経([0-9.]+)度");
            if (match.Success)
            {
                occuredLongitude = double.Parse(match.Groups[2].Value);
                if (match.Groups[1].Value == "西")
                    occuredLongitude *= -1;
            }

            return occuredLongitude;
        }

        private int getOccuredDepth()
        {
            string html = getHtml(true, true, true, true, true);
            int occuredDepth = -1;

            string[] patterns = new string[] {
                @"深さは?、?約?(\d+|ごく浅い|ごく浅く)(キロ|km)?",
                @"深さは?、?「?約?(ごく浅い|ごく浅く)",
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(html, pattern);
                if (match.Success)
                {
                    if (Regex.IsMatch(match.Groups[1].Value, "ごく浅い|ごく浅く"))
                        occuredDepth = 0;
                    else
                        occuredDepth = int.Parse(match.Groups[1].Value);
                    break;
                }
            }

            return occuredDepth;
        }

        private double getOccuredMagnitude()
        {
            string html = getHtml(true, true, true, true, true);
            double occuredMagnitude = -1;

            Match match = Regex.Match(html, @"マグニチュード[)）]?は(\d+\.\d)と");
            if (match.Success)
            {
                occuredMagnitude = double.Parse(match.Groups[1].Value);
            }

            return occuredMagnitude;
        }

        private int getOccuredScale()
        {
            string html = getHtml(true, true, true, true, true);
            int occuredScale = -1;

            MatchCollection matches = Regex.Matches(html, @"震度は?([1-47]|[5-6]([弱強]))(?!以上)"); // (\d)(弱|強)?(?!以上)");
            foreach (Match match in matches)
            {
                // SCALE CONVERT
                int detectionScale = int.Parse(match.Groups[1].Value[0].ToString()) * 10;
                if (match.Groups[1].Length == 2)
                    if (match.Groups[1].Value[1] == '弱')
                        detectionScale -= 5;

                // SCALE COMPARE
                if (detectionScale > occuredScale)
                    occuredScale = detectionScale;
            }

            return occuredScale;
        }

        private DomesticTsunamiType getDomesticTsunamiType()
        {
            string html = getHtml(true, true, true, true);
            DomesticTsunamiType type = DomesticTsunamiType.Unknown;
            IssueType issueType = getIssueType();

            Dictionary<string, DomesticTsunamiType> searchTable
                = new Dictionary<string, DomesticTsunamiType>();

            if (issueType == IssueType.Foreign)
            {
                searchTable.Add(@"津波(警報|予報).{1,15}発表(中|しています)", DomesticTsunamiType.Warning);
                searchTable.Add(@"津波注意報.{1,15}発表(中|しています)", DomesticTsunamiType.Watch);
                searchTable.Add(@"若干の海面変動がある", DomesticTsunamiType.NonEffective);
                searchTable.Add(@"日本への津波.{3,12}調査中", DomesticTsunamiType.Checking);
                searchTable.Add(@"日本への津波の(心配|影響)はありません", DomesticTsunamiType.None);
                searchTable.Add(@"津波の心配はありません", DomesticTsunamiType.None);
            }
            else
            {
                searchTable.Add(@"津波(警報|予報).{1,15}発表(中|しています)", DomesticTsunamiType.Warning);
                searchTable.Add(@"津波注意報.{1,15}発表(中|しています)", DomesticTsunamiType.Watch);
                searchTable.Add(@"多少の海面変動が(ある|続く|続き)", DomesticTsunamiType.NonEffective);
                searchTable.Add(@"若干の海面変動がある", DomesticTsunamiType.NonEffective);
                searchTable.Add(@"今後の情報に注意", DomesticTsunamiType.Checking);
                searchTable.Add(@"津波の心配はありません", DomesticTsunamiType.None);
            }

            foreach (string key in searchTable.Keys)
            {
                if (Regex.IsMatch(html, key))
                {
                    type = searchTable[key];
                    break;
                }
            }

            return type;
        }

        private ForeignTsunamiType getForeignTsunamiType()
        {
            string html = getHtml(true, true, true, true);
            ForeignTsunamiType type = ForeignTsunamiType.Unknown;
            IssueType issueType = getIssueType();

            if (issueType == IssueType.Foreign)
            {
                Dictionary<string, ForeignTsunamiType> searchTable
                    = new Dictionary<string, ForeignTsunamiType>();

                searchTable.Add(@"太平洋の(広域|広範囲|広い範囲)に津波", ForeignTsunamiType.WarningPacificWide);
                searchTable.Add(@"インド洋の(広域|広範囲|広い範囲)に津波", ForeignTsunamiType.WarningIndianWide);
                searchTable.Add(@"太平洋で津波", ForeignTsunamiType.WarningPacific);
                searchTable.Add(@"インド洋で津波", ForeignTsunamiType.WarningIndian);
                searchTable.Add(@"震源の近傍で津波", ForeignTsunamiType.WarningNearby);
                searchTable.Add(@"津波が発生することが", ForeignTsunamiType.Potential);
                searchTable.Add(@"震源の近傍で小さな津波", ForeignTsunamiType.NonEffectiveNearby);
                searchTable.Add(@"この地震による津波の心配はありません", ForeignTsunamiType.None);

                foreach (string key in searchTable.Keys)
                {
                    if (Regex.IsMatch(html, key))
                    {
                        type = searchTable[key];
                        break;
                    }
                }
            }

            return type;
        }

        private CorrectType getCorrectType()
        {
            string html = getHtml(true, true, true, true);
            CorrectType type = (getIssueType() == IssueType.Other ? CorrectType.Unknown : CorrectType.None);

            Dictionary<string, CorrectType> searchTable
                = new Dictionary<string, CorrectType>();

            searchTable.Add(@"震源(要素)?.{1,3}震度を訂正する", CorrectType.ScaleAndDestination);
            searchTable.Add(@"震度.{1,3}震源(要素?)を訂正する", CorrectType.ScaleAndDestination);
            searchTable.Add(@"震度を訂正する", CorrectType.ScaleOnly);
            searchTable.Add(@"震源(要素)?を訂正する", CorrectType.DestinationOnly);
            //searchTable.Add(@"(?!付加文.+)訂正する", CorrectType.Unknown);

            foreach (string key in searchTable.Keys)
            {
                if (Regex.IsMatch(html, key))
                {
                    type = searchTable[key];
                    break;
                }
            }

            return type;
        }

        private IList<ObservationPoint> getObservationPoints()
        {
            string html = getHtml(false, false, false, false, false);

            // preのあいだをいただきたい．
            return (new HTML2ObservationPoint()).Do(html, getIssueType());
        }

        // TODO: getProtocolTime: プロトコル時刻を取得するようになっていない
        private DateTime getProtocolTime(double offset)
        {
            DateTime issueTime = getIssueTime();
            if (issueTime == DateTime.MinValue)
                issueTime = DateTime.Now;

            return issueTime.AddSeconds(offset);
        }

        // 以下，ルーチン
        string sourceHtml;
        string[] processedHtml; 

        private void init(string html)
        {
            sourceHtml = html;
            processedHtml = new string[32];
        }

        private string getHtml(bool hasCutCrLf, bool hasCutTag, bool hasCutHeadBlank, bool hasCutBlank)
        {
            return getHtml(hasCutCrLf, hasCutTag, hasCutHeadBlank, hasCutBlank, false);
        }

        private string getHtml(bool hasCutCrLf, bool hasCutTag, bool hasCutHeadBlank, bool hasCutBlank, bool hasConvertFullNum2Half)
        {
            int flag = 0x00;

            flag |= (hasCutCrLf ? 0x01 : 0x00);
            flag |= (hasCutTag ? 0x02 : 0x00);
            flag |= (hasCutHeadBlank ? 0x04 : 0x00);
            flag |= (hasCutBlank ? 0x08 : 0x00);
            flag |= (hasConvertFullNum2Half ? 0x10 : 0x00);

            if (processedHtml[flag] == null)
            {
                processedHtml[flag] = sourceHtml;

                if (hasCutTag)
                    processedHtml[flag] = cutTag(processedHtml[flag]);
                if (hasCutHeadBlank)
                    processedHtml[flag] = cutHeadBlank(processedHtml[flag]);
                if (hasCutCrLf)
                    processedHtml[flag] = cutCrLf(processedHtml[flag]);
                if (hasCutBlank)
                    processedHtml[flag] = cutBlank(processedHtml[flag]);
                if (hasConvertFullNum2Half)
                    processedHtml[flag] = fullNum2Half(processedHtml[flag]);
            }

            return processedHtml[flag];
        }

        private string cutCrLf(string html)
        {
            return html.Replace("\r", "").Replace("\n", "");
        }

        private string cutTag(string html)
        {
            return Regex.Replace(html, "<(\"[^\"]*\"|'[^']*'|[^'\">])*?>", "", RegexOptions.Multiline);
        }

        private string cutHeadBlank(string html)
        {
            return Regex.Replace(html, "^[ 　	]*", "", RegexOptions.Multiline);
        }

        private string cutBlank(string html)
        {
            html = Regex.Replace(html, @"[ 　	]*", "", RegexOptions.Multiline);
            html = Regex.Replace(html, @"&nbsp;?", "", RegexOptions.Multiline);
            return html;
        }
        
        private string fullNum2Half(string includeString)
        {
            includeString = Regex.Replace(includeString, "[０-９]", new MatchEvaluator( num2Half ));
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
