using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess
{
	public class SQLiteDbSql
	{
		public const string CREATETABLE_PHOTO =
			@"CREATE TABLE IF NOT EXISTS Photo (
				POI_ID INTEGER, 
				Order_ID INTEGER, 
				URI TEXT)";
		public const string CREATETABLE_REVIEW =
			@"CREATE TABLE IF NOT EXISTS Review (
				Id INTEGER PRIMARY KEY,
				UserId INTEGER, 
				MerchantId INTEGER, 
				StarLvl TINYINT, 
				DateTime DATETIME, 
				Content TEXT, 
				Remark TEXT);";

		public const string ALTERTABLE_REVIEW =
			@"ALTER TABLE IF EXISTS Review (
				Id INTEGER PRIMARY KEY,
				UserId INTEGER, 
				MerchantId INTEGER, 
				StarLvl TINYINT, 
				DateTime DATETIME, 
				Content TEXT, 
				Remark TEXT);";
		public const string CREATETABLE_USER =
			@"CREATE TABLE IF NOT EXISTS User (
				Id INTEGER PRIMARY KEY, 
				Name TEXT, 
				Email TEXT);";
		public const string CREATETABLE_SIGN =
            @"CREATE TABLE IF NOT EXISTS Sign (
				POI_ID INTEGER, 
				UserId INTEGER, 
				SignCount INTEGER);";
	}
}

