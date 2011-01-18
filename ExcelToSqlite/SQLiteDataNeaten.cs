using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.SQLite;
using System.Data;

namespace ExcelToSqlite
{
	public class SQLiteDataNeaten
	{
		public static void Neaten()
		{
			DbPath.InitSQLiteDb();
			string dianpingId = "3446332";
			string name = MerchantAccess.GetPOI_Name(dianpingId);
			Console.WriteLine("{0} {1}", name, dianpingId);
			string where = "StringNum = '{0}'";
			DataTable dt = MerchantAccess.GetPOI(where, dianpingId);
			DbPath.DisplayTable(dt);
			where = "Latitude is null or Longitude is null";
			dt = MerchantAccess.GetPOI(where);
			DbPath.DisplayTable(dt);
			/******************************
			dianpingId = "1769029";
			double lat = 40.4685;
			string updatelat = MerchantAccess.GetUpdateSql(dianpingId, "lat", lat.ToString());
			MerchantAccess.UpdateMerchant(updatelat);
			dianpingId = "3446332";
			MerchantRoutine.Delete(dianpingId);
			dianpingId = "2729446";
			MerchantRoutine.Delete(dianpingId);
			MerchantRoutine.Neaten();
			*******************************/
			dt = MerchantAccess.GetAllPOI();
			DbPath.DisplayTable(dt);
		}

		internal static void NeatenPhoneNo()
		{
			DbPath.InitSQLiteDb();
			DataTable dt = MerchantAccess.GetAllPOI();
			DisplayPhoneNo(dt);
		}
		public static void DisplayPhoneNo(DataTable dt)
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow row = dt.Rows[i];
				string colname = dt.Columns["Phone"].ColumnName;
				string no = row["Phone"].ToString();
				string city = row["City"].ToString();
				object id = row["Id"];
				/*************************************
				 * 删除两个以上电话号码只留一个
				string newNo = no.Substring(0, no.IndexOf(' ')).TrimEnd('手', '机', ':');
				if (no.Contains(" "))
				{
					MerchantAccess.UpdateMerchant(string.Format("UPDATE Merchant SET Phone = '{0}' WHERE Id = {1}", newNo, id));
					DisplayColumn(i, colname, no, newNo);
				}
				**************************************/
				/*************************************
				 * 添加城市区号
				string newNo = null;
				if (!(no.StartsWith("0") || no.StartsWith("4") || no.StartsWith("(")))
				{
					if (city.Contains("Beijing"))
					{
						newNo = no.Insert(0, "010-");
					}
					if (city.Contains("Shanghai"))
					{
						newNo = no.Insert(0, "021-");
					} 
					MerchantAccess.UpdateMerchant(string.Format("UPDATE Merchant SET Phone = '{0}' WHERE Id = {1}", newNo, id));
					DisplayColumn(i, colname, no, newNo);
				}
				**************************************/
				/*************************************
				 * 删除以转和字母结尾的电话号码的末尾不合法字符
				string newNo = no.TrimEnd('转', 'Q');
				if (!newNo.Equals(no))
				{
					MerchantAccess.UpdateMerchant(string.Format("UPDATE Merchant SET Phone = '{0}' WHERE Id = {1}", newNo, id));
					DisplayColumn(i, colname, no, newNo);
				}
				**************************************/
			}
		}

		private static void DisplayColumn(int i, string colname, string no, string newNo)
		{
			Console.WriteLine("{0} ------------------------------------------", i);
			Console.Write("[{0, 12}]", colname);
			Console.Write("[{0}]", newNo);
			Console.WriteLine("[{0}]", no);
		}
	}
}
