using System;
using System.Collections.Generic;
using P2PQuake.JMAInformation;
using P2PQuake.JMAInformation.Quake.Convert.Receive;

namespace QuakeReceiver
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Launch at " + DateTime.Now.ToString ());
            Console.WriteLine ("Version " + System.Reflection.Assembly.GetExecutingAssembly().GetName ().Version.ToString());

            // URL管理
            UrlManager manager = new UrlManager();

            // URL展開
            UrlExtractor extractor = new UrlExtractor();
            List<string> urls = extractor.ExtractUrls();

            // ご新規さんを抽出
            List<string> newUrls = manager.selectNonExists(urls);

            Console.WriteLine ("  total: " +  urls.Count + ", new: " + newUrls.Count);

            // ダウンロード
            PageDownloader downloader = new PageDownloader();

            foreach (string url in newUrls)
            {
                System.Threading.Thread.Sleep (10000);

                Console.WriteLine ("  new address: " + url);

                string fileName = downloader.Download(url);
                if (fileName != null)
                    manager.addUrl (url);
                else
                    Console.WriteLine ("    ...fail!!");
            }

            Console.WriteLine ("Finish at " + DateTime.Now.ToString());
        }
    }
}
