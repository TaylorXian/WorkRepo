using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataAccess.SQLite;
using DataAccess;
using System.Data;

namespace ExcelToSqlite
{
	public class DbPath
	{
		public static string GetDbFilepath(string dbFilepath)
		{
			if (!File.Exists(dbFilepath))
			{
				Console.WriteLine("file {0} doesn't exist.", dbFilepath);
				dbFilepath = string.Format("../../../Data{0}", dbFilepath.TrimStart('.'));
				Console.WriteLine("filepath change to {0}", dbFilepath);
			}
			return GetFullFilePath(dbFilepath);
		}

        public static string InitSQLiteDb()
        {
            string dbpath = DbPath.GetDbFilepath("./sqlite.db");
            SQLiteAccess.ConnStringSQLite = SQLiteAccess.GetConnectionString(dbpath);
            return dbpath;
        }

        public static string GetFullFilePath(string relativepath)
        {
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativepath));
        }

		public static void DisplayTable(DataTable dt)
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				Console.WriteLine("{0} ------------------------------------------", i);
				for (int j = 0; j < dt.Columns.Count; j++)
				{
					Console.Write("[{0, 12}]", dt.Columns[j].ColumnName);
					Console.WriteLine("[{0}]", dt.Rows[i][j]);
				}
			}
		}
	}
}
