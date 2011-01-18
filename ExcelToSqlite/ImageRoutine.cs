using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataAccess.SQLite;
using DataAccess.Excel;
using System.Data;
using System.Threading;
using DataAccess;

namespace ExcelToSqlite
{
	public class ImageRoutine
	{
		public static void GenerateImageDbAndFile()
		{
			// Read Excel Table
			string dbFilepath = DbPath.GetDbFilepath("./sqlite.db");
			string curDir = Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), dbFilepath)));
			Console.WriteLine(curDir);
			// if dbFilepath is relative path, below statement output null. 
			// because method Path.GetPathRoot(dbFilepath) return null.
			Console.WriteLine(Path.GetPathRoot(dbFilepath));
			SQLiteAccess.ConnStringSQLite = SQLiteAccess.GetConnectionString(dbFilepath);
			SQLiteAccess.Execute(SQLiteDbSql.CREATETABLE_PHOTO);
			IList<string> xlsPaths = new List<string>();
			xlsPaths.Add(DbPath.GetDbFilepath("./POIInformation_excel2003(6).xls"));
			xlsPaths.Add(DbPath.GetDbFilepath("./Dianping_Photo_Derek1.xls"));
			xlsPaths.Add(DbPath.GetDbFilepath("./Dianping_Photo_Derek2.xls"));
			foreach (var xlsFilepath in xlsPaths)
			{
				ExcelAccess.XLSConnectionString = ExcelAccess.GetConnectionString(xlsFilepath);
				foreach (DataRow rowSheet in ExcelAccess.GetSheetTables().Rows)
				{
					string tblName = rowSheet["Table_Name"].ToString();
					Console.WriteLine(tblName);
					DownloadImgInSheet(tblName, curDir);
				}
			}
		}

		private static void DownloadImgInSheet(string tblName, string curFullDir)
		{
			string logpath = LogPath.GetLogfilepath(curFullDir);
			switch (tblName)
			{
				case "Sheet1$":
				case "Photo$":
					{
						string select = string.Format("SELECT * FROM [{0}];", tblName);
						DataSet ds = ExcelAccess.ReadTable(select);
						DataTable dt = ds.Tables[0];
						foreach (DataRow row in dt.Rows)
						{
							int iPOI_ID = MerchantAccess.GetPOI_ID(row["Dianping ID"].ToString());
							if (iPOI_ID > 0)
							{
								for (int i = 1; i < dt.Columns.Count; i++)
								{
									if (row[i] != null)
									{
										string imgAddr = row[i].ToString().Trim();
										if (!string.IsNullOrEmpty(imgAddr))
										{
											Thread.Sleep(5 * 1000);
											int orderId;
											string filename;
											string uri;
											string localImagepath = "";
											string insert = "";
											try
											{
												orderId = GetOrder_ID(dt.Columns[i].ColumnName);
												filename = imgAddr.Substring(imgAddr.LastIndexOf("/") + 1);
												uri = string.Format("./POI/Photos/{0}/{1}", iPOI_ID, filename);
												localImagepath = Path.GetFullPath(Path.Combine(curFullDir, uri));
												if (!Directory.Exists(Path.GetDirectoryName(localImagepath)))
												{
													Directory.CreateDirectory(Path.GetDirectoryName(localImagepath));
												}
												if (GetImageFileFromHttpAddress(imgAddr, localImagepath))
												{
													insert = string.Format("INSERT INTO Photo(POI_ID, Order_ID, URI) VALUES({0}, {1}, '{2}');", iPOI_ID, orderId, uri.TrimStart('.'));
													SQLiteAccess.Execute(insert);
												}
											}
											catch (Exception ex)
											{
												File.AppendAllText(logpath, iPOI_ID.ToString() + localImagepath);
												File.AppendAllText(logpath, "\r\n");
												File.AppendAllText(logpath, ex.Message);
												File.AppendAllText(logpath, "\r\n");
												continue;
											}
											Console.WriteLine(iPOI_ID);
											Console.WriteLine(localImagepath);
											Console.WriteLine(imgAddr);
											File.AppendAllText(logpath, localImagepath);
											File.AppendAllText(logpath, "\r\n");
											File.AppendAllText(logpath, insert);
											File.AppendAllText(logpath, "\r\n");
										}
									}
								}
							}
						}
						break;
					}
				default:
					break;
			}
		}

		private static bool GetImageFileFromHttpAddress(string imgAddr, string localimgFullpath)
		{
			if (!File.Exists(localimgFullpath))
			{
				HttpWorker.HttpProc.HttpGetFile(imgAddr, localimgFullpath);
			}
			return true;
		}

		private static int GetOrder_ID(string colname)
		{
			int orderId = 0;
			string newColname = GetNewColname(colname);
			if (int.TryParse(newColname, out orderId))
			{
				orderId--;
				//Console.WriteLine(colname + " " +
				//    newColname + "orderId is " + orderId);
			}
			return orderId;
		}

		private static string GetNewColname(string colname)
		{
			switch (colname)
			{
				case "F2":
				case "F3":
				case "F4":
				case "F5":
				case "F6":
				case "F7":
				case "F8":
				case "F9":
					return colname.TrimStart('F');
				case "F1":
				default:
					break;
			}
			return colname;
		}

	}
}
