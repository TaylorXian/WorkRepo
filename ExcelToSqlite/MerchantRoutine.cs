using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess.SQLite;
using DataAccess.Excel;
using System.IO;

namespace ExcelToSqlite
{
    public class MerchantRoutine
    {
        public static void ExcelTableToSQLite()
        {
            // set dbfile position
            string dbFilepath = DbPath.GetDbFilepath("./POIInformation_excel2003(6)");
            Console.WriteLine(Path.GetPathRoot(dbFilepath));
            ExcelAccess.XLSConnectionString = ExcelAccess.GetConnectionString(dbFilepath);
            DbPath.InitSQLiteDb();
            // Query
            // 
            string sql = "SELECT * FROM [完整版$]";
            DataSet ds = ExcelAccess.ReadTable(sql);
            DataTable dt = ds.Tables[0];
            //for (int i = 0; i < dt.Columns.Count; i++)
            //{
            //    Console.WriteLine(dt.Columns[i].ColumnName);
            //}
            // Set 第二级分类 和 第三级分类 为 null
            MerchantAccess.UpdateMerchant("UPDATE Merchant SET Tags = null");
            MerchantAccess.UpdateMerchant("UPDATE Merchant SET Tags3 = null");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Console.WriteLine(i);
                string dianpingId = dt.Rows[i]["Dianping ID"].ToString();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    object fieldvalue = dt.Rows[i][j];
                    if (fieldvalue == null)
                    {
                        continue;
                    }
                    double d = 0.0;
                    string fieldname = dt.Columns[j].ColumnName;
                    if (double.TryParse(fieldvalue.ToString(), out d))
                    {
                        if (fieldname.Contains("long") && d < 90.0)
                        {
                            fieldname = fieldname.Replace("long", "lat");
                        }
                        if (fieldname.Contains("lat") && d > 90.0)
                        {
                            fieldname = fieldname.Replace("lat", "long");
                        }
                    }
                    string sqlSqlite = MerchantAccess.GetUpdateSql(dianpingId, fieldname, fieldvalue.ToString());
                    if (!string.IsNullOrEmpty(sqlSqlite))
                    {
                        MerchantAccess.UpdateMerchant(sqlSqlite);
                        Console.WriteLine(sqlSqlite);
                    }
                    //Console.Write(dt.Rows[i][j]);
                    //Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        internal static void Delete(string dianpingId)
        {
            int id = MerchantAccess.GetPOI_ID(dianpingId);
            if (id > 0)
            {
                string delete = string.Format("delete from merchant where id = {0};", id);
                SQLiteAccess.Execute(delete);
            }
        }

        private static string NeatenDesc(string des)
        {
            return DelBlankString(des);
        }

        private static string NeatenRecommended(string rec)
        {
            return DelBlankString(rec.Replace("网友推荐", "").Replace("取消", ""));
        }

        private static string DelBlankString(string src)
        {
            return src.Trim().TrimStart(' ', '\r', '\n');
        }

        internal static void Neaten()
        {
            DataTable dt = MerchantAccess.GetAllPOI();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Console.WriteLine(i);
                DataRow row = dt.Rows[i];
                string newDesc = NeatenDesc(row["Desc"].ToString());
                string newRec = NeatenRecommended(row["Recommended"].ToString().Replace("(null)", ""));
                if (!string.IsNullOrEmpty(newDesc.Trim()))
                {
                    MerchantAccess.UpdateMerchant(string.Format("UPDATE Merchant SET Desc = '{0}' WHERE Id = {1};", newDesc, row["Id"]));
                }
                if (!string.IsNullOrEmpty(newRec.Trim()))
                {
                    MerchantAccess.UpdateMerchant(string.Format("UPDATE Merchant SET Recommended = '{0}' WHERE Id = {1};", newRec, row["Id"]));
                }
            }
        }
    }
}
