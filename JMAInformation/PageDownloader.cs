using System;
using System.Net;
using System.IO;

namespace P2PQuake.JMAInformation
{
    /// <summary>
    /// ページを収集・保存するクラス
    /// </summary>
    public class PageDownloader
    {
        private string saveDirectory;

        public PageDownloader()
        {
            string directory     = System.IO.Path.GetDirectoryName(
                                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            string saveDirectory = System.IO.Path.Combine (directory, "downloads");

            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            this.saveDirectory = saveDirectory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Receiver.PageCollector"/> class.
        /// </summary>
        /// <param name='saveDirectory'>
        /// 保存先ディレクトリ
        /// </param>
        public PageDownloader(string saveDirectory)
        {
            if (!Directory.Exists(saveDirectory))
                throw new DirectoryNotFoundException("指定されたディレクトリ " + saveDirectory + " が見つかりません。");
            this.saveDirectory = saveDirectory;
        }
        
        /// <summary>
        /// 指定したURLのデータを取得し，インスタンス生成時に指定されたディレクトリに保存します．
        /// 保存ファイル名は自動的に決定されます．
        /// </summary>
        /// <returns>
        /// 成功: 保存ファイル名
        /// 失敗: null
        /// </returns>
        /// <param name='url'>
        /// 取得するURL
        /// </param>
        public string Download(string url) 
        {
            Uri    uri      = new Uri(url);
            string fileName = uri.Segments[uri.Segments.Length-1];

            if (Download(url, fileName))
                return fileName;
            else
                return null;
        }
        
        /// <summary>
        /// 指定したURLのデータを取得し，インスタンス生成時に指定されたディレクトリに保存します．
        /// </summary>
        /// <returns>
        /// 取得・保存に成功したか否か
        /// </returns>
        /// <param name='url'>
        /// 取得するURL
        /// </param>
        /// <param name='fileName'>
        /// 保存ファイル名
        /// </param>
        public bool Download(string url, string fileName)
        {
            string fullPath = Path.Combine(saveDirectory, fileName);
            WebClient client = new WebClient();
            
            client.DownloadFile(new Uri(url), fullPath);
            return true;
        }
    }
}

