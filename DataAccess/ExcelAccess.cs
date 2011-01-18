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
	}
}
