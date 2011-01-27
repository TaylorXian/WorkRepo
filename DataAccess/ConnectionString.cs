using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
	public class ConnectionString
	{
		public const string EXCEL =
			"Provider=Microsoft.Jet.OLEDB.4.0;" +
			"Data Source=POIInformation_excel2003(5).xls;" +
			"Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
		public const string EXCEL_TEMPLATE =
			"Provider=Microsoft.Jet.OLEDB.4.0;" +
			"Data Source={0};" +
			"Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
		public const string SQLITE_TEMPLATE =
			"Data Source={0}";
		public const string SQLITE =
			"Data Source=./sqlite.db";
		public const string MYSQL_TEMPLATE =
			"DRIVER=MySQL ODBC 3.51 Driver;" +
			"Server={0};Port=3306;Option=16384;Pooling=true;" +
			"Stmt=SET NAMES 'gbk';" +
			"Database={1};Uid={2};Pwd={3};";
	}
	public enum MyDbType
	{
		EXCEL, SQLite, MySQL
	}
}
