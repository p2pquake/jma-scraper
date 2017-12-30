using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace P2PQuake.JMAInformation.Tsunami.Convert
{
    public class JMATsunamiConverter
    {
        public static Core fromString(string content)
        {
            // 解析方法
            // 1. PREタグを抽出する
            // 2. 異なる方法で抽出する
            //   a. 「発表状況」セクションを全解析する
            //      ※新規発表については該当セクションなし（本文セクション）
            //   b. 「＜XX＞」から空行までを該当発表エリアとして抽出する
            // 3. 差異があった場合は警告扱い

            int begin = content.IndexOf("<pre");
            int end = content.IndexOf("</pre");

            if (begin < 0 || end < 0)
            {
                Console.WriteLine("取得できなかったためスキップします.");
                return null;
            }

            string body = content.Substring(begin, end - begin);

            // 情報種が予期されないものは除去する
            if (Regex.IsMatch(body, "津波観測に関する情報|津波に関するその他の情報|津波到達予想時刻|予想される津波の高さ"))
            {
                Console.WriteLine("情報種が異なるためスキップ: " + body.Substring(0,60));
                return null;
            }
            
            // *****
            // 解析ロジック
            // *****
            Core core = new Core();
            core.issue = new Issue();
            core.issue.type = "Focus";
            core.areas = new List<Area>();

            // 発表情報
            Match issueFromMatch = Regex.Match(body, "^(.+時.{1,2}分)[ 　]+(.+)発表", RegexOptions.Multiline);
            if (issueFromMatch.Success)
            {
                core.issue.time = issueFromMatch.Groups[1].Value;
                core.issue.source = issueFromMatch.Groups[2].Value;
            }

            // 全データサーチ
            string[] lines = Regex.Replace(body, "\r", "").Split('\n');
            string level = "";

            foreach (string line in lines)
            {
                if (line == "" || line.Contains("＊＊"))
                {
                    level = "";
                    continue;
                }

                if (line.StartsWith("＜"))
                {
                    if (Regex.IsMatch(line, "＜(津波|津波警報|津波の津波警報)＞"))
                    {
                        level = "津波警報";
                    }
                    if (Regex.IsMatch(line, "＜(大津波|大津波警報|大津波の津波警報)＞"))
                    {
                        level = "大津波警報";
                    }
                    if (Regex.IsMatch(line, "＜(津波注意|津波注意報)＞"))
                    {
                        level = "津波注意報";
                    }
                    if (Regex.IsMatch(line, "＜津波予報（若干の海面変動）＞"))
                    {
                        level = "若干の海面変動";
                    }

                    continue;
                }

                if (!line.StartsWith(" ") && !line.StartsWith("　"))
                {
                    level = "";
                    continue;
                }

                if (level != "")
                {
                    string[] areas = line.Trim(new char[] { ' ', '　' }).Split('、').Where(e => e != "").ToArray();
                    foreach (string area in areas)
                    {
                        Area tsunamiArea = new Tsunami.Area();
                        tsunamiArea.grade = level;
                        tsunamiArea.name = Regex.Replace(area, "[＊＄＃]", "");
                        
                        if (area.Contains("＊"))
                        {
                            tsunamiArea.immediate = true;
                        }

                        // 既に存在していた場合、末尾に移動
                        if (core.areas.Any(e => e.name == tsunamiArea.name))
                        {
                            Area existArea = core.areas.First(e => e.name == tsunamiArea.name);
                            core.areas.Remove(existArea);
                            core.areas.Add(existArea);
                            continue; 
                        }

                        core.areas.Add(tsunamiArea);
                    }
                }
            }
            
            if (core.areas.Count == 0)
            {
                core.cancelled = true;
            }

            Console.WriteLine(body);
            return core;
        }
    }
}
