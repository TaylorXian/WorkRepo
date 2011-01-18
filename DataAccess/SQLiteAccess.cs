using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace DataAccess.SQLite
{
	public class SQLiteAccess
	{
		private static string connStringSQLite;

		public static string ConnStringSQLite
		{
			get
			{
				if (string.IsNullOrEmpty(connStringSQLite))
				{
					throw new Exception("SQLite ConnectionString uninitialized!");
					ConnStringSQLite = ConnectionString.SQLITE;
					return connStringSQLite;
				}
				return connStringSQLite;
			}
			set { connStringSQLite = value; }
		}

		public static string GetConnectionString(string filepath)
		{
			if (string.IsNullOrEmpty(filepath.Trim()))
			{
				return ConnectionString.SQLITE;
			}
			return string.Format(ConnectionString.SQLITE_TEMPLATE, filepath);
		}

		public static void Execute(string sql)
		{
			SQLiteConnection connSQlite = null;
			using (connSQlite = new SQLiteConnection(ConnStringSQLite))
			{
				connSQlite.Open();
				SQLiteCommand cmdSqlite = new SQLiteCommand(sql, connSQlite);
				cmdSqlite.ExecuteNonQuery();
				connSQlite.Close();
			}
		}

		public static object ExecuteScalar(string sql)
		{
			object reVal = null;
			SQLiteConnection connSQlite = null;
			using (connSQlite = new SQLiteConnection(ConnStringSQLite))
			{
				connSQlite.Open();
				SQLiteCommand cmdSqlite = new SQLiteCommand(sql, connSQlite);
				reVal = cmdSqlite.ExecuteScalar();
				connSQlite.Close();
			}
			return reVal;
		}

        public static DataTable ExecuteVector(string select)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(ConnStringSQLite))
            {
                conn.Open();
                SQLiteDataAdapter da = new SQLiteDataAdapter(select, conn);
                da.Fill(dt);
                conn.Close();
            }
            return dt;
        }
    }
}
