using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace QuakeReceiver.URLManager
{
    public class UrlUpdateManager
    {
        /// <summary>
        /// 管理ファイル名
        /// </summary>
        string fileName;

        private SQLiteConnection con = null;
        private SQLiteCommand    cmd = null;

        public UrlUpdateManager ()
        {
            string directory = System.IO.Path.GetDirectoryName(
                                System.Reflection.Assembly.GetExecutingAssembly().Location);
            string baseName  = "UrlUpdateManager.sqlite3";
            string fileName  = System.IO.Path.Combine (directory, baseName);

            this.fileName    = fileName;
            initialize ();
        }

        public UrlUpdateManager(string fileName)
        {
            this.fileName = fileName;
            initialize ();
        }

        public long[] getLastModified(string url)
        {
            long[] result = new long[2];

            open();
            try
            {
                cmd.CommandText = "SELECT last_modified FROM urls WHERE url = '" + url + "'";
                result[0] = Convert.ToInt64(cmd.ExecuteScalar());
                cmd.CommandText = "SELECT last_checked FROM urls WHERE url = '" + url + "'";
                result[1] = Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            close();

            return result;
        }

        public void setLastModified(string url, long last_modified, long last_checked)
        {
            open();
            try
            {
                cmd.CommandText = "REPLACE INTO urls VALUES ('" + url + "', " + last_modified + ", " + last_checked + ");";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
            }
            close();
        }

        private void initialize()
        {
            open ();

            try {
                cmd.CommandText = "CREATE TABLE urls (url TEXT PRIMARY KEY, last_modified INTEGER, last_checked INTEGER)";
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

