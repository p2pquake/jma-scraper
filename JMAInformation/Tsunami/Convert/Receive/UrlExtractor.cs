using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;

namespace P2PQuake.JMAInformation.Tsunami.Convert.Receive
{
    /// <summary>
    /// 気象庁 津波情報の情報一覧からURLを抽出するクラス
    /// </summary>
    public class UrlExtractor
    {
        // 気象庁 津波情報の一覧ページURL
        private string[] urls = {
#if LOCAL
            "http://www.p2pquake.net/test/list.html",
#else
            "http://www.jma.go.jp/jp/tsunami/list.html", // 津波予報一覧
#endif
        };
            
        public UrlExtractor ()
        {
        }
        
        /// <summary>
        /// 津波情報のURLを抽出し，絶対パスで返します．
        /// </summary>
        /// <returns>
        /// URLリスト
        /// </returns>
        public List<string> ExtractUrls()
        {
            List<string> list = new List<string>();
            
            foreach (string url in urls) {
                //System.Threading.Thread.Sleep (10000);
                list.AddRange(ExtractUrls(url)); 
            }
            
            return list;
        }
        
        /// <summary>
        /// 津波情報のURLを抽出し，絶対パスで返します．
        /// </summary>
        /// <returns>
        /// URLリスト
        /// </returns>
        /// <param name='url'>
        /// 一覧ページのURL
        /// </param>
        public List<string> ExtractUrls(string url) {
            List<string> list = new List<string>();
           
            // ページを取得し，HtmlDocument(Html Agility Pack)として初期化
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            
            // リンクを抽出し，絶対パスに変換してリストに
            Uri uri = new Uri(url);
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@class='infotable']/table//tr//td[1]//a");
            
            if (nodes != null)
            {
                foreach (HtmlNode link in nodes)
                {
                    Uri linkUri = new Uri(uri, link.Attributes["href"].Value);
                    list.Add(linkUri.AbsoluteUri);
                }
            }
            
            return list;
        }
    }
}

