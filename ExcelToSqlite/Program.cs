﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataAccess;
using DataAccess.Excel;
using DataAccess.SQLite;
using NPOI.HSSF.UserModel;
using System.IO;
using HttpWorker;

namespace ExcelToSqlite
{
	class Program
	{
		static void Main(string[] args)
		{
			//ImageRoutine.GenerateImageDbAndFile();
			//ReviewRoutine.GeneralReviewData();
            //MerchantRoutine.ExcelTableToSQLite();
			//SQLiteDataNeaten.Neaten();
			//DatabaseAccess da = DatabaseAccess.Create(MyDbType.MySQL,
			//    "220.113.40.161", "robot", "root", "lenci");
			//DbPath.DisplayTable(da.ExecuteVector("select * from category where robot_id = 'dianping';"));
			//SQLiteDataNeaten.NeatenPhoneNo();
			TranslateRoutine.StartTrans();
			//string content = "新年快乐";
			//content = IcibaTrans.Trans(content);
			//Console.WriteLine(content);
			Console.ReadKey();
		}
	}
}
