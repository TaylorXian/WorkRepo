using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.SQLite;
using System.Data;

namespace ExcelToSqlite
{
	public class TranslateRoutine
	{
		internal static void StartTrans()
		{
			DbPath.InitSQLiteDb();
			DataTable dt = SQLiteAccess.ExecuteVector("select Id, StringNum, Name FROM Merchant");
			DbPath.DisplayTable(dt);
		}
		public static void SaveTrans()
		{
			DbPath.GetFullFilePath(
		}
	}
}
