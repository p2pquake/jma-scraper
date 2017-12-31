using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace TsunamiReceiver.URLManager
{
    public class UrlManager
    {
        /// <summary>
        /// 管理ファイル名
        /// </summary>
        string fileName;

        private SQLiteConnection con = null;
        private SQLiteCommand    cmd = null;

        public UrlManager ()
        {
            string directory = System.IO.Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().Location);
            string baseName  = "UrlManager.sqlite3";
            string fileName  = System.IO.Path.Combine (directory, baseName);

            this.fileName    = fileName;
            initialize ();
        }

        public UrlManager(string fileName)
        {
            this.fileName = fileName;
            initialize ();
        }

        public List<string> selectNonExists(List<string> urls)
        {
            List<string> nonExistUrls = new List<string>();

            foreach (string url in urls)
            {
                if (!exist (url))
                    nonExistUrls.Add (url);
            }

            return nonExistUrls;
        }

        public bool exist(string url)
        {
            bool result = false;

            open ();
            try {
                cmd.CommandText = "SELECT COUNT(*) FROM urls WHERE url = '" + url + "'";
                int count = Convert.ToInt32 (cmd.ExecuteScalar());

                result = count > 0;
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
            close ();

            return result;
        }

        public bool addUrl(string url)
        {
            open ();
            try {
                cmd.CommandText = "INSERT INTO urls VALUES ('" + url + "')";
                cmd.ExecuteNonQuery();
            } catch (Exception e) { }
            close ();

            return true;
        }

        private void initialize()
        {
            open ();

            try {
                cmd.CommandText = "CREATE TABLE urls (url TEXT PRIMARY KEY)";
                cmd.ExecuteNonQuery();
            } catch (SQLiteException e) { }

            close ();
        }

        private void open()
        {
            con = new SQLiteConnection("Data Source=" + fileName);
            cmd = con.CreateCommand();
            con.Open();
        }

        private void close()
        {
            try {
                con.Close ();
            }
            catch (Exception e)
            {
                Console.WriteLine ("close error: " + e.StackTrace);
            }
        }
    }
}

