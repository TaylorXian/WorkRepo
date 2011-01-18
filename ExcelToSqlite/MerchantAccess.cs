using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.SQLite;
using System.Data;

namespace ExcelToSqlite
{
    public class MerchantAccess
    {
        public static int GetPOI_ID(string dianpingId)
        {
            string selectPOI_ID = string.Format("SELECT Id FROM Merchant WHERE StringNum = '{0}';", dianpingId);
            object id = SQLiteAccess.ExecuteScalar(selectPOI_ID);
            if (id == null)
            {
                return 0;
            }
            return Convert.ToInt32(id);
        }

        public static string GetPOI_Name(string dianpingId)
        {
            return GetMerchantColumnValue(dianpingId, "Name");
        }

        internal static DataTable GetPOI(string where, params object[] objs)
        {
            if (!string.IsNullOrEmpty(where.Trim()))
            {
                string select = string.Format("SELECT * FROM Merchant WHERE {0};", where);
                if (objs != null &&
                    objs.Length > 0)
                {
                    try
                    {
                        select = string.Format(select, objs);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                return SQLiteAccess.ExecuteVector(select);
            }
            throw new Exception("can't find where clause.");
        }
        internal static DataTable GetAllPOI()
        {
            string select = "SELECT * FROM Merchant;";
            return SQLiteAccess.ExecuteVector(select);
        }
        public static void UpdateMerchant(string sql)
        {
            SQLiteAccess.Execute(sql);
            Console.WriteLine(sql);
        }

        public static string GetMerchantColumnValue(string dianpingId, string fieldname)
        {
            string update = string.Format("SELECT {0} FROM Merchant Where StringNum = '{1}';",
                fieldname, dianpingId);
            object obj = SQLiteAccess.ExecuteScalar(update);
            Console.WriteLine(update);
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }

        private static string GetNewAttrValue(string dianpingId, string fieldname, string fieldvalue)
        {
            string oldAttrValue = MerchantAccess.GetMerchantColumnValue(dianpingId, fieldname);
            string newAttrValue = null;
            if (oldAttrValue.Contains("null") || string.IsNullOrEmpty(oldAttrValue.Trim()))
            {
                newAttrValue = fieldvalue;
            }
            else
            {
                newAttrValue = string.Format("{0} {1}", oldAttrValue.Trim(), fieldvalue);
            }
            return newAttrValue;
        }

        public static string GetUpdateSql(string dianpingId, string fieldname, string fieldvalue)
        {
            if (string.IsNullOrEmpty(dianpingId) ||
                string.IsNullOrEmpty(fieldname) ||
                string.IsNullOrEmpty(fieldvalue))
            {
                return null;
            }
            if (fieldname.Contains("Category") || fieldname.Contains("第一分类"))
            {
                return string.Format("UPDATE Merchant SET {0} = '{1}' WHERE StringNum = '{2}'",
                    "Category", fieldvalue, dianpingId);
            }
            else if (fieldname.Contains("Tags") || fieldname.Contains("第二分类"))
            {
                string newAttrValue = GetNewAttrValue(dianpingId, "Tags", fieldvalue);
                return string.Format("UPDATE Merchant SET {0} = '{1}' WHERE StringNum = '{2}'",
                    "Tags", newAttrValue, dianpingId);
            }
            else if (fieldname.Contains("第三分类"))
            {
                string newAttrValue = GetNewAttrValue(dianpingId, "Tags3", fieldvalue);
                return string.Format("UPDATE Merchant SET {0} = '{1}' WHERE StringNum = '{2}'",
                    "Tags3", newAttrValue, dianpingId);
            }
            else if (fieldname.Contains("Longitude") || fieldname.Contains("long"))
            {
                return string.Format("UPDATE Merchant SET {0} = {1} WHERE StringNum = '{2}'",
                    "Longitude", fieldvalue, dianpingId);
            }
            else if (fieldname.Contains("Latitude") || fieldname.Contains("lat"))
            {
                return string.Format("UPDATE Merchant SET {0} = {1} WHERE StringNum = '{2}'",
                    "Latitude", fieldvalue, dianpingId);
            }
            return null;
        }
    }
}
