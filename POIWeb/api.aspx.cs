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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Collections.Generic;

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
					ErrJsonMsg("unfound get params!");
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
		/// 3 done
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
				string slt = string.Format(SQLiteDbSql.GET_REVIEW, poiid, c, s, userid);
				DataTable dt = SQLiteAccess.ExecuteVector(slt);
				string dataString = DataToJson(dt);
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
				Response.Write(string.Format("{{ result:\"success\", totalCount:\"{0}\", start:\"{1}\", count:\"{2}\", [{3}] }}", dt.Rows.Count, s, c, dataString));
			}
			catch (Exception ex)
			{
				ErrJsonMsg(ex.Message);
			}
		}

		private string DataToJson(DataTable dt)
		{
			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in dt.Rows)
			{
				sb.Append("{ ");
				foreach (DataColumn col in dt.Columns)
				{
					sb.AppendFormat("{0}:{1}, ", col.ColumnName, StringToJson(row[col.ColumnName].ToString()));
				}
				if (sb.Length > 2)
				{
					sb.Remove(sb.Length - 2, 1);
				}
				sb.Append(" },");
			}
			if (sb.Length > 0)
			{
				sb.Remove(sb.Length - 1, 1);
			}
			return sb.ToString();
		}

		private static string StringToJson(string s)
		{
			var serializer = new DataContractJsonSerializer(s.GetType());
			var stream = new MemoryStream();
			serializer.WriteObject(stream, s);
			byte[] dataBytes = new byte[stream.Length];
			stream.Position = 0;
			stream.Read(dataBytes, 0, (int)stream.Length);
			stream.Close();
			return Encoding.UTF8.GetString(dataBytes);
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
				if (cnt == 0)
				{
					cnt++;
					if (InsertSignCount(poiid, userid, cnt))
					{
						Response.Write(string.Format("{{ result:\"success\", signCount:\"{0}\" }}", cnt));
					}
					else
					{
						ErrJsonMsg("InsertSignCount error!");
					}
				}
				else if (cnt > 0)
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

		private bool InsertSignCount(string poiid, string userid, int cnt)
		{
			string insert = string.Format("INSERT INTO Sign(POI_ID, UserId, SignCount) VALUES({0}, {1}, {2});", poiid, userid, cnt);
			return MyExecute(insert);
		}

		private bool UpdateSignCount(string poiid, string userid, int cnt)
		{
			string upt = string.Format("UPDATE Sign SET SignCount = {2} where POI_ID = {0} AND UserId = {1};", poiid, userid, cnt);
			return MyExecute(upt);
		}

		private bool MyExecute(string sql)
		{
			try
			{
				SQLiteAccess.Execute(sql);
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
			//Response.Write(msg);
		}

		private void ErrJsonMsg(string msg)
		{
			Response.Write(string.Format("{{ result:\"failure\", error:\"{0}\" }}", msg));
		}

		private int GetSignCount(string poiid, string userid)
		{
			string slt = string.Format("select SignCount from sign where POI_ID = {0} AND UserId = {1};", poiid, userid);
			return MyExecuteScalar(slt);
		}

		private int GetSignCount(string poiid)
		{
			string slt = string.Format("SELECT sum(SignCount) AS cnt FROM Sign Where POI_ID = {0}", poiid);
			return MyExecuteScalar(slt);
		}

		private int MyExecuteScalar(string slt)
		{
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
			int cnt = GetSignCount(poiid);
			if (cnt < 0)
			{
				ErrJsonMsg("no such user!");
				//ErrJsonMsg("GetSignCount error!");
			}
			else
			{
				//TODO: getsigncount
				Response.Write(string.Format("{{ result:\"success\", signCount:\"{0}\" }}", cnt));
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
