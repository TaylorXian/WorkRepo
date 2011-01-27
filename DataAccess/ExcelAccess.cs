using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace DataAccess.Excel
{
	public class ExcelAccess
	{
		private static string xlsConnectionString;

		/// <summary>
		/// 使用oledb读写excel出现"操作必须使用一个可更新的查询"的解决办法使用oledb读写excel出现“操作必须使用一个可更新的查询”的解决办法
		/// Extended Properties='Excel 8.0;HDR=yes;IMEX=1'
		/// A： HDR ( HeaDer Row )设置
		/// 若指定值为Yes，代表 Excel 档中的工作表第一行是栏位名称
		/// 若指定值為 No，代表 Excel 档中的工作表第一行就是資料了，沒有栏位名称
		/// B：IMEX ( IMport EXport mode )设置
		/// IMEX 有三种模式，各自引起的读写行为也不同，容後再述：
		/// 0 is Export mode
		/// 1 is Import mode
		/// 2 is Linked mode (full update capabilities)
		/// 我这里特别要说明的就是 IMEX 参数了，因为不同的模式代表著不同的读写行为：
		/// 当IMEX=0 时为"汇出模式"，这个模式开启的 Excel 档案只能用来做"写入"用途。
		/// 当 IMEX=1 时为"汇入模式"，这个模式开启的 Excel 档案只能用来做"读取"用途。
		/// 当 IMEX=2 时为"连結模式"，这个模式开启的 Excel 档案可同时支援"读取"与"写入"用途。
		/// </summary>
		public static string XLSConnectionString
		{
			get
			{
				if (string.IsNullOrEmpty(xlsConnectionString))
				{
					ExcelAccess.XLSConnectionString = ConnectionString.EXCEL;
					return xlsConnectionString;
				}
				return xlsConnectionString;
			}
			set { ExcelAccess.xlsConnectionString = value; }
		}

		public static string GetConnectionString(string filepath)
		{
			if (string.IsNullOrEmpty(filepath.Trim()))
			{
				return ConnectionString.EXCEL;
			}
			return string.Format(ConnectionString.EXCEL_TEMPLATE, filepath);
		}
		
		public static DataSet ReadTable(string select)
		{
			// Query
			OleDbConnection conn = null;
			DataSet ds = new DataSet();
			using (conn = new OleDbConnection(XLSConnectionString))
			{
				conn.Open();
				OleDbDataAdapter da =
					new OleDbDataAdapter(select, conn);
				da.Fill(ds);
				conn.Close();
			};

			return ds;
		}

		public static DataTable GetSheetTables()
		{
			OleDbConnection conn = null;
			DataTable dt = new DataTable();
			using (conn = new OleDbConnection(XLSConnectionString))
			{
				conn.Open();
				dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
				conn.Close();
			};

			return dt;
		}

		public static void Excute(string sql)
		{
			OleDbConnection conn = null;
			using (conn = new OleDbConnection(XLSConnectionString))
			{
				conn.Open();
				OleDbCommand cmd = new OleDbCommand(sql, conn);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}

		public static void Excute(string sql, string contentT)
		{
			OleDbConnection conn = null;
			using (conn = new OleDbConnection(XLSConnectionString))
			{
				conn.Open();
				OleDbCommand cmd = new OleDbCommand(sql, conn);
				OleDbParameter p = new OleDbParameter("@Content", contentT);
				p.OleDbType = OleDbType.VarChar;
				//p.Size = 10000;
				cmd.Parameters.Add(p);
				cmd.ExecuteNonQuery();
				conn.Close();
			}
		}

		public static object ExcuteScalar(string sql)
		{
			object reVal = null;
			OleDbConnection conn = null;
			using (conn = new OleDbConnection(XLSConnectionString))
			{
				conn.Open();
				OleDbCommand cmd = new OleDbCommand(sql, conn);
				reVal = cmd.ExecuteScalar();
				conn.Close();
			}
			return reVal;
		}
	}
}
