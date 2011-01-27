using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.SQLite;
using System.Data;
using DataAccess.Excel;
using Translate;
using org.in2bits.MyXls;
using NPOI.HSSF.UserModel;
using System.IO;

namespace ExcelToSqlite
{
	public class TranslateRoutine
	{
		internal static void StartTrans()
		{
			string dbpath = DbPath.InitExcelDb("./Translate.xls");
			DbPath.InitSQLiteDb();
			DataTable dt = SQLiteAccess.ExecuteVector("select Id, StringNum, Name, Desc, Recommended FROM Merchant");
			TranslateMerchantInfo(dt);

			dt = SQLiteAccess.ExecuteVector("select Id, Content from review;");
			TranslateReview(dt);
		}

		private static void TranslateReview(DataTable dt)
		{
			HSSFWorkbook workbook = new HSSFWorkbook();
			HSSFSheet s = workbook.CreateSheet("Review") as HSSFSheet;
			int i = 2;
			int iR = 0;
			foreach (DataRow row in dt.Rows)
			{
				string id = row["Id"].ToString();
				string content = row["Content"].ToString();
				string contentT = MyTrans(content);
				HSSFRow rXls = s.CreateRow(iR++) as HSSFRow;
				HSSFCell cId = rXls.CreateCell(0, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cId.SetCellValue(id);
				HSSFCell cCon = rXls.CreateCell(1, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cCon.SetCellValue(contentT);
				i++;
			}
			FileStream fs = new FileStream(@"Review.xls", FileMode.OpenOrCreate);
			workbook.Write(fs);
			fs.Close();
		}

		private static void TranslateMerchantInfo(DataTable dt)
		{
			HSSFWorkbook workbook = new HSSFWorkbook();
			HSSFSheet s = workbook.CreateSheet("Merchant") as HSSFSheet;
			int iR = 0;
			HSSFRow rHdr = s.CreateRow(iR++) as HSSFRow;
			foreach (DataRow row in dt.Rows)
			{
				string id = row["Id"].ToString();
				string dianpingId = row["StringNum"].ToString();
				string name = row["Name"].ToString();
				string desc = row["Desc"].ToString();
				string recom = row["Recommended"].ToString();
				string nameT = MyTrans(name);
				string descT = MyTrans(desc);
				string recomT = MyTrans(recom);
				HSSFRow rXls = s.CreateRow(iR++) as HSSFRow;
				HSSFCell cId = rXls.CreateCell(0, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cId.SetCellValue(id);
				HSSFCell cDPId = rXls.CreateCell(1, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cDPId.SetCellValue(dianpingId);
				HSSFCell cZhName = rXls.CreateCell(2, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cZhName.SetCellValue(name);
				HSSFCell cName = rXls.CreateCell(3, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cName.SetCellValue(nameT);
				HSSFCell cDesc = rXls.CreateCell(4, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cDesc.SetCellValue(descT);
				HSSFCell cRe = rXls.CreateCell(5, NPOI.SS.UserModel.CellType.STRING) as HSSFCell;
				cRe.SetCellValue(recomT);

				//SaveTrans(id, dianpingId, nameT, descT, recomT);
			}
			FileStream fs = new FileStream(@"Merchant.xls", FileMode.OpenOrCreate);
			workbook.Write(fs);
			fs.Close();
		}

		private static void SaveReviewTrans(string id, string contentT)
		{
			string select = "select count(*) from [评论$] where id = '{0}';";
			object obj = ExcelAccess.ExcuteScalar(string.Format(select, id));
			int cnt = 0;
			if (obj != null && int.TryParse(obj.ToString(), out cnt) && cnt > 0)
			{
				//string update = "update [评论$] SET 内容 = '{0}' WHERE id = '{1}';";
				//ExcelAccess.Excute(string.Format(update, contentT, id));
				string updatePara = string.Format("update [评论$] SET 内容 = @Content WHERE id = '{0}';", id);
				ExcelAccess.Excute(updatePara, contentT);
			}
			else
			{
				//string insert = "Insert into [评论$](id, 内容) values('{0}', '{1}');";
				//ExcelAccess.Excute(string.Format(insert, id, contentT));
				string insert = string.Format("Insert into [评论$](id, 内容) values('{0}', @Content);", id);
				ExcelAccess.Excute(insert, contentT);
				string insertId = string.Format("Insert into [评论$](id) values('{0}');", id);
				ExcelAccess.Excute(insertId);
			}
		}

		private static string MyTrans(string text)
		{
			if (!string.IsNullOrEmpty(text) && !text.Contains("null"))
			{
				string txt = NeatenText(text);
				StringBuilder sb = new StringBuilder();
				string[] ts = txt.Split('。', '？', '！');
				foreach (string s in ts)
				{
					if (!string.IsNullOrEmpty(s.Trim()))
					{
						int time = 15;
					trans :
						string t = MyTranslate.TranslateY(s);
						if (t == null)
						{
							System.Threading.Thread.Sleep(time++ * 1000);
							//if (time < 30)
							//{
							//    goto trans;
							//}
						}
						else
						{
							t = t.Replace("\\", "");
							sb.Append(t);
							Console.WriteLine(t);
							//sb.Append('.');
						}
					}
				}
				return sb.ToString();
			}
			return string.Empty;
		}

		private static string NeatenText(string text)
		{
			StringBuilder txt = new StringBuilder(text);
			int idx = text.IndexOf("推荐菜");
			if (idx > 0)
			{
				txt.Insert(idx, "。");
			}
			//txt.Replace('！', '。');
			//txt.Replace('！', '。');
			txt.Replace('，', ' ');
			return txt.ToString();
		}

		/// <summary>
		/// Excel Create Table 不能使用 if exists 子句。
		/// 使用下面语句创建工作薄后，字段名称前面有单引号：
		/// Create Table mySheet ([id] TEXT(32), [dianping_id] TEXT(16), [名称] TEXT(255), [描述] TEXT(255), [推荐] TEXT(255));
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dianpingId"></param>
		/// <param name="objs"></param>
		public static void SaveTrans(string id, string dianpingId, params object[] objs)
		{
			string select = "select count(*) from [Sheet1$] where id = '{0}' and dianping_id = '{1}';";
			object obj = ExcelAccess.ExcuteScalar(string.Format(select, id, dianpingId));
			int cnt = 0;
			if (obj != null && int.TryParse(obj.ToString(), out cnt) && cnt > 0)
			{
				string[] attrNames = { "名称", "描述", "推荐" };
				int i = 0;
				foreach (string s in objs)
				{
					UpdateTrans(id, dianpingId, attrNames[i++], s);
				}
			}
			else
			{
				string insert = "Insert into [Sheet1$](id, dianping_id, 名称, 描述, 推荐) values('{0}', '{1}', '{2}', '{3}', '{4}');";
				string i = string.Format(insert, id, dianpingId, objs[0], objs[1], objs[2]);
				ExcelAccess.Excute(i);
			}
		}

		private static void UpdateTrans(string id, string dianpingId, string attrName, string attrVal)
		{
			if (!string.IsNullOrEmpty(attrVal))
			{
				string update = "update [Sheet1$] SET {0} = '{1}' WHERE id = '{2}' AND dianping_id = '{3}';";
				ExcelAccess.Excute(string.Format(update, attrName, attrVal, id, dianpingId));
			}
		}
	}
}
