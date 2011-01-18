using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DataAccess.SQLite;
using DataAccess.Excel;
using System.Data;
using System.Threading;
using HttpWorker;
using DataEntity;
using DataAccess;

namespace Test.Proj
{
	class Program
	{
		static void Main(string[] args)
		{
			// Download Image File OK.
			/*  */
			Console.WriteLine(Environment.CurrentDirectory);
			Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
			Console.WriteLine(Directory.GetCurrentDirectory());
			Console.WriteLine();
            //HtmlPage page = new HtmlPage("http://www.dianping.com/shop/1953784", "./log.txt");
            //IList<Review> reviews = page.GetReviwList();
            //Console.ReadKey();
			Type t = typeof(ConnectionString);
			Type tt = ConnectionString.EXCEL.GetType();
		}
	}
}
