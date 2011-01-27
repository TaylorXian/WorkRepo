using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using DataAccess.SQLite;
using DataAccess;

namespace POIWeb
{
    public partial class api : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //act=getsign&poiid=123
            string act = GetReqParam("act");
            InitSqliteDb();
            switch (act)
            {
                case "getsign":
                    GetSign();
                    break;
                case "sign":
                    Sign();
                    break;
                case "getreview":
                    GetReview();
                    break;
                case "review":
                    AddReview();
                    break;
                default:
                    ErrJsonMsg("unknown action error!");
                    break;
            }
        }

        /// <summary>
        /// 4
        /// </summary>
        private void AddReview()
        {
            string poiid = GetReqParam("poiid");
            string userid = GetReqParam("userid");
            string sl = GetReqParam("starlevel");
            string c = GetReqParam("content");
            if (MyStringNull(poiid, userid, sl, c))
            {
                ErrJsonMsg("Request params may be null!");
                return;
            }
            string dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string insert = string.Format(@"INSERT INTO Review(
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
						'{5}');", userid, poiid, sl, c, dt, string.Empty);
            try
            {
                SQLiteAccess.Execute(insert);
                Response.Write("{ result:\"success\" }");
            }
            catch (Exception ex)
            {
                ErrJsonMsg(ex.Message);
            }
        }

        /// <summary>
        /// 3
        /// </summary>
        private void GetReview()
        {
            string poiid = GetReqParam("poiid");
            string userid = GetReqParam("userid");
            string s = GetReqParam("start");
            string c = GetReqParam("count");
            if (MyStringNull(poiid, userid, s, c))
            {
                ErrJsonMsg("Request params may be null!");
                return;
            }
            try
            {
                //TODO: GetReviews
                //string slt = string.Format("select * from review where merchantid = {0} and userid = {1}  limit {2} offset {3};", poiid, userid, c, s);
                string slt = string.Format("select User.Id AS userid, User.Name AS username, StarLvl AS starlevel, DateTime AS date, Content as content from Review, User where merchantid = {0} and Userid = User.id limit {1} offset {2};", poiid, c, s, userid);
                DataTable dt = SQLiteAccess.ExecuteVector(slt);
                    //{
                    //    userid:"123",
                    //    username:"liutao",
                    //    starlevel:"8",
                    //    date:"2010-11-01",
                    //    content:"very good!"
                    //},
                    //{
                    //    userid:"123",
                    //    username:"liutao",
                    //    starlevel:"8",
                    //    date:"2010-11-01",
                    //    content:"very good!"
                    //},                   
                Response.Write(string.Format("{{ result:\"success\", totalCount:\"{0}\", start:\"{1}\", count:\"{2}\", [{3}] }}", dt.Rows.Count, s, c));
            }
            catch (Exception ex)
            {
                ErrJsonMsg(ex.Message);
            }
        }
        /// <summary>
        /// 2
        /// </summary>
        private void Sign()
        {
            string poiid = GetReqParam("poiid");
            string userid = GetReqParam("userid");
            if (MyStringNull(poiid, userid))
            {
                ErrJsonMsg("Request params may be null!");
                return;
            }
            if (MyExists("poi", poiid) && 
                MyExists("user", userid))
            {
                //TODO: sign
                int cnt = GetSignCount(poiid, userid);
                if (cnt >= 0)
                {
                    cnt++;
                    if (UpdateSignCount(poiid, userid, cnt))
                    {
                        Response.Write(string.Format("{{ result:\"success\", signCount:\"{0}\" }}", cnt));
                    }
                    else
                    {
                        ErrJsonMsg("UpdateSignCount error!");
                    }
                }
                else
                {
                    ErrJsonMsg("GetSignCount error!");
                }
            }
            else
            {
                ErrJsonMsg("no such user or poi!");
            }
        }

        private bool UpdateSignCount(string poiid, string userid, int cnt)
        {
            string upt = string.Format("UPDATE Sign SET SignCount = {2} where POI_ID = {0} AND UserId = {1};", poiid, userid, cnt);
            try
            {
                SQLiteAccess.Execute(upt);
                return true;
            }
            catch (Exception ex)
            {
                ErrMsg(ex.Message);
            }
            return false;
        }

        private void ErrMsg(string msg)
        {
            Response.Write(msg);
        }

        private void ErrJsonMsg(string msg)
        {
            Response.Write(string.Format("{{ result:\"failure\", error:\"{0}\" }}", msg));
        }

        private int GetSignCount(string poiid, string userid)
        {
            string slt = string.Format("select SignCount from sign where POI_ID = {0} AND UserId = {1};", poiid, userid);
            try
            {
                object obj = SQLiteAccess.ExecuteScalar(slt);
                int cnt = 0;
                if (obj != null && int.TryParse(obj.ToString(), out cnt))
                {
                    return cnt;
                }
                return 0;
            }
            catch (Exception ex)
            {
                ErrMsg(ex.Message);
            }
            return -1;
        }

        /// <summary>
        /// 1
        /// </summary>
        private void GetSign()
        {
            string poiid = GetReqParam("poiid");
            string userid = GetReqParam("userid");
            if (MyStringNull(poiid))
            {
                ErrJsonMsg("Request params may be null!");
                return;
            }
            string selectSign = string.Format("SELECT sum(signcount) AS cnt FROM Sign Where POI_ID = {0}", poiid);
            object obj = SQLiteAccess.ExecuteScalar(selectSign);
            int cnt = 0;
            if (obj != null && int.TryParse(obj.ToString(), out cnt) && cnt > 0)
            {
                //TODO: getsigncount
                cnt = GetSignCount(poiid, userid);
                if (cnt >= 0)
                {
                    Response.Write(string.Format("{{ result:\"success\", signCount:\"{0}\" }}", cnt));
                }
                else
                {
                    ErrJsonMsg("GetSignCount error!");
                }
            }
            else
            {
                ErrJsonMsg("no such user!");
            }
        }

        private void InitSqliteDb()
        {
            string strConn = Server.MapPath("~/App_Data/sqlite.db");
            SQLiteAccess.ConnStringSQLite = SQLiteAccess.GetConnectionString(strConn);
            SQLiteAccess.Execute(SQLiteDbSql.CREATETABLE_SIGN);
        }

        private bool MyExists(string s, string id)
        {
            string select = null;
            switch (s)
            {
                case "user":
                    select = string.Format("select count(*) from user where id = {0}", id);
                    break;
                case "poi":
                    select = string.Format("select count(*) from merchant where id = {0}", id);
                    break;
                default:
                    return false;
            }
            object obj = SQLiteAccess.ExecuteScalar(select);
            int cnt = 0;
            if (obj != null && int.TryParse(obj.ToString(), out cnt) && cnt > 0)
            {
                return true;
            }
            return false;
        }

        private string GetReqParam(string name)
        {
            return Request.Params[name];
        }

        private bool MyStringNull(params string[] strs)
        {
            if (strs.Length > 0)
            {
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s.Trim()))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
    }
}
