using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.SQLite;
using System.Data.OleDb;

namespace DataAccess
{
	public class DatabaseAccess
	{
		private string connectionString;
		private MyDbType dbType;
		private DatabaseAccess()
		{

		}

		public static DatabaseAccess Create(MyDbType dbType, params object[] objs)
		{
			switch (dbType)
			{
				case MyDbType.EXCEL:
					break;
				case MyDbType.SQLite:
					break;
				case MyDbType.MySQL:
					DatabaseAccess da = new DatabaseAccess();
					da.dbType = dbType;
					da.connectionString = string.Format(ConnectionString.MYSQL_TEMPLATE, objs);
					return da;
				default:
					break;
			}
			return null;
		}

		public DataTable ExecuteVector(string select)
		{
			DataSet ds = new DataSet();
			IDataAdapter da = GetDataAdapter(select);
			da.Fill(ds);
			return ds.Tables[0];
		}

		private IDbConnection GetConnection()
		{
			switch (dbType)
			{
				case MyDbType.EXCEL:
					return CreateDbConnection<OleDbConnection>();
				case MyDbType.SQLite:
					return CreateDbConnection<SQLiteConnection>();
				case MyDbType.MySQL:
					return CreateDbConnection<OdbcConnection>();
				default:
					break;
			}
			throw new Exception("unknown the database type.");
		}

		private IDataAdapter GetDataAdapter(string command)
		{
			switch (dbType)
			{
				case MyDbType.EXCEL:
					return new OleDbDataAdapter(command, connectionString);
				case MyDbType.SQLite:
					return new SQLiteDataAdapter(command, connectionString);
				case MyDbType.MySQL:
					return new OdbcDataAdapter(command, connectionString);
				default:
					break;
			}
			throw new Exception("unknown the database type.");
		}

		private IDbConnection CreateDbConnection<T>() where T : IDbConnection, new()
		{
			IDbConnection t = new T();
			t.ConnectionString = connectionString;
			return t;
		}
	}
}
