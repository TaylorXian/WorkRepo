using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Threading;
using HttpWorker;
using System.Net;
using System.Web;

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
			
			//Console.ReadKey();
		}

		private static void TranslateTest()
		{
			string respFilename = "./header.txt";
			StreamWriter sw = new StreamWriter(File.Create(respFilename));
			//string origin = HttpUtility.UrlDecode(TestUrl.URL1);
			//sw.WriteLine(origin); 
			////sw.WriteLine(TestUrl.URL1);
			//sw.WriteLine(HttpUtility.UrlEncode("上帝").ToUpper());
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(TestUrl.URL3);
			req.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
			WebResponse resp = req.GetResponse();
			for (int i = 0; i < resp.Headers.Count; i++)
			{
				sw.WriteLine("{0} {1}", resp.Headers.Keys[i], resp.Headers.Get(i));
			}
			Stream stream = resp.GetResponseStream();
			Console.WriteLine(resp.ContentType);
			StreamReader sr = new StreamReader(stream);
			sw.WriteLine(sr.CurrentEncoding);
			sw.WriteLine(sr.ReadToEnd());
			sr.Close();
			sw.Close();
		}
	}
}
