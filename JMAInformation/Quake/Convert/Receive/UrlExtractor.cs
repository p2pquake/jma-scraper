using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;

namespace P2PQuake.JMAInformation.Quake.Convert.Receive
{
    /// <summary>
    /// 気象庁 地震情報の情報一覧からURLを抽出するクラス
    /// </summary>
    public class UrlExtractor
    {
        // 気象庁 地震情報の一覧ページURL
        private string[] urls = {
            "http://www.jma.go.jp/jp/quake/quake_sindo_index.html",     // 震度速報
            "http://www.jma.go.jp/jp/quake/quake_singen_index.html",    // 震源
            "http://www.jma.go.jp/jp/quake/quake_singendo_index.html",  // 震度・震源
            "http://www.jma.go.jp/jp/quake/quake_local_index.html",     // 各地の震度
            "http://www.jma.go.jp/jp/quake/quake_foreign_index.html",   // 遠地地震
            "http://www.jma.go.jp/jp/quake/quake_sonota_index.html",    // その他
        };
            
        public UrlExtractor ()
        {
        }
        
        /// <summary>
        /// 地震情報のURLを抽出し，絶対パスで返します．
        /// </summary>
        /// <returns>
        /// URLリスト
        /// </returns>
        public List<string> ExtractUrls()
        {
            List<string> list = new List<string>();
            
            foreach (string url in urls) {
                System.Threading.Thread.Sleep (10000);
                list.AddRange(ExtractUrls(url)); 
            }
            
            return list;
        }
        
        /// <summary>
        /// 地震情報のURLを抽出し，絶対パスで返します．
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
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//div[@id='info']/table//tr//td[1]//a");
            
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

