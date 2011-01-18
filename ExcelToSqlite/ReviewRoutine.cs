using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpWorker;
using DataEntity;
using System.IO;
using DataAccess.SQLite;
using DataAccess;
using System.Threading;

namespace ExcelToSqlite
{
    public class ReviewRoutine
    {
        static string dbpath;
        static string logpath;
        public static void GeneralReviewData()
        {
            dbpath = DbPath.InitSQLiteDb();
            SQLiteAccess.Execute(SQLiteDbSql.CREATETABLE_REVIEW);

            string filepath = DbPath.GetFullFilePath(DbPath.GetDbFilepath("./dianping.bj.txt"));
            ProcessReview(filepath);

            filepath = DbPath.GetFullFilePath(DbPath.GetDbFilepath("./dianping.sh.txt"));
            ProcessReview(filepath);
        }

        private static string GetLogpath()
        {
            if (!string.IsNullOrEmpty(dbpath.Trim()))
            {
                return LogPath.GetLogfilepath(Path.GetDirectoryName(dbpath));
            }
            throw new Exception("SQLite file path uninitialized!");
        }

        private static void ProcessReview(string filepath)
        {
            logpath = GetLogpath();
            StreamReader sr = new StreamReader(filepath);

            string dianpingId = sr.ReadLine();
            while (!string.IsNullOrEmpty(dianpingId))
            {
                string merchantUrl = "http://www.dianping.com/shop/" + dianpingId;
                File.AppendAllText(logpath, merchantUrl);
                File.AppendAllText(logpath, "\r\n");
                HtmlPage page = new HtmlPage(merchantUrl, logpath);
                IList<Review> reviews = page.GetReviwList();
                ProcessSQLiteDb(reviews, dianpingId);
                Thread.Sleep(3 * 1000);
                dianpingId = sr.ReadLine();
            }
            sr.Close();
        }

        private static void ProcessSQLiteDb(IList<Review> reviews, string dianpingId)
        {
            int mId = MerchantAccess.GetPOI_ID(dianpingId);
            if (mId > 0)
            {
                foreach (Review r in reviews)
                {
                    r.MerchantId = mId;
                    try
                    {
                        AddReview(r);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(logpath, ex.Message);
                        File.AppendAllText(logpath, "\r\n");
                    }
                }
            }
        }

        private static void AddReview(Review r)
        {
            int uId = UpdateUser(r.Username);
            int rId = GetReviewId(r);
            if (rId > 0)
            {
                string update = string.Format(
                    @"UPDATE Review 
						SET StarLvl = {1}, 
						Content = '{2}', 
						DateTime = '{3}' 
					WHERE Id = {0};", rId,
                                r.StarLevel,
                                r.Content,
                                r.PostTime);
                SQLiteAccess.Execute(update);
            }
            else
            {
                string insert = string.Format(
                    @"INSERT INTO Review(
						UserId, 
						MerchantId, 
						StarLvl, 
						Content, 
						DateTime, 
						Remark) 
					VALUES(
						{0}, 
						{1}, 
						{2}, 
						'{3}', 
						'{4}', 
						'{5}');",
                                uId.ToString(),
                                r.MerchantId.ToString(),
                                r.StarLevel,
                                r.Content,
                                r.PostTime,
                                string.Empty);
                SQLiteAccess.Execute(insert);
            }
        }

        private static int GetReviewId(Review r)
        {
            string selectId = string.Format("SELECT Id FROM Review WHERE UserId = {0} AND MerchantId = {1};", GetUserId(r.Username), r.MerchantId);
            object obj = SQLiteAccess.ExecuteScalar(selectId);
            if (obj == null)
            {
                return 0;
            }
            return Convert.ToInt32(obj);
        }

        private static int UpdateUser(string p)
        {
            int uId = GetUserId(p);
            if (uId > 0)
            {
                return uId;
            }

            return AddUser(p);
        }

        private static int AddUser(string p)
        {
            return AddUser(p, string.Empty);
        }

        private static int AddUser(string p, string email)
        {
            string insert = string.Format("INSERT INTO User(Name, Email) VALUES('{0}', '{1}');", p, email);
            SQLiteAccess.Execute(insert);
            return GetUserId(p);
        }

        private static int GetUserId(string p)
        {
            string selectId = string.Format("SELECT Id FROM User WHERE Name = '{0}';", p);
            object obj = SQLiteAccess.ExecuteScalar(selectId);
            if (obj == null)
            {
                return 0;
            }
            return Convert.ToInt32(obj);
        }
    }
}
