using System;
using System.Collections.Generic;
using System.Net;
using P2PQuake.JMAInformation;
using P2PQuake.JMAInformation.Tsunami.Convert.Receive;
using TsunamiReceiver.URLManager;

namespace TsunamiReceiver
{
    class Program
    {
        private static readonly string JMA_TSUNAMI_INDEX_URL = "https://www.jma.go.jp/jp/tsunami/";

        public static void Main(string[] args)
        {
            Console.WriteLine("Launch at " + DateTime.Now.ToString());
            Console.WriteLine("Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

            // 実施判定
            if (!hasToCheckUpdate())
            {
                return;
            }

            // URL管理
            UrlManager manager = new UrlManager();

            // URL展開
            UrlExtractor extractor = new UrlExtractor();
            List<string> urls = extractor.ExtractUrls();

            // ご新規さんを抽出
            List<string> newUrls = manager.selectNonExists(urls);

            Console.WriteLine("  total: " + urls.Count + ", new: " + newUrls.Count);

            // ダウンロード
            PageDownloader downloader = new PageDownloader();

            foreach (string url in newUrls)
            {
                System.Threading.Thread.Sleep(1000);

                Console.WriteLine("  new address: " + url);

                string fileName = downloader.Download(url);
                if (fileName != null)
                    manager.addUrl(url);
                else
                    Console.WriteLine("    ...fail!!");
            }

            Console.WriteLine("Finish at " + DateTime.Now.ToString());
        }

        /// <summary>
        /// 新規情報をチェックすべきかどうか判断します
        /// </summary>
        /// <returns></returns>
        private static bool hasToCheckUpdate()
        {
            UrlUpdateManager urlUpdateManager = new UrlUpdateManager();
            long[] urlUpdateInfo = urlUpdateManager.getLastModified(JMA_TSUNAMI_INDEX_URL);

            Console.Error.WriteLine("    現在日時: " + DateTime.Now);
            Console.Error.WriteLine("最終確認日時: " + DateTime.FromBinary(urlUpdateInfo[1]));

            // 更新確認から3分以内か？
            if (DateTime.Now.Subtract(DateTime.FromBinary(urlUpdateInfo[1])).TotalSeconds < 200)
            {
                return true;
            }

            long lastModified = retreiveIndexLastModified();

            Console.Error.WriteLine("更新日時(WEB): " + DateTime.FromBinary(lastModified));
            Console.Error.WriteLine("更新日時(DB ): " + DateTime.FromBinary(urlUpdateInfo[0]));

            // 更新されているか？
            if (DateTime.FromBinary(lastModified) > DateTime.FromBinary(urlUpdateInfo[0]))
            {
                urlUpdateManager.setLastModified(JMA_TSUNAMI_INDEX_URL, lastModified, DateTime.Now.ToBinary());
                return true;
            }

            return false;
        }

        /// <summary>
        /// インデックスページの最終更新日時を取得する
        /// </summary>
        /// <returns></returns>
        private static long retreiveIndexLastModified()
        {
            WebRequest webRequest = WebRequest.Create(JMA_TSUNAMI_INDEX_URL);
            webRequest.Method = "HEAD";
            WebResponse webResponse = webRequest.GetResponse();

            DateTime lastModified = DateTime.MinValue;
            foreach (string key in webResponse.Headers.Keys)
            {
                if (key.ToLower() == "last-modified")
                {
                    lastModified = DateTime.Parse(webResponse.Headers[key]);
                }
            }

            return lastModified.ToBinary();
        }
    }
}
