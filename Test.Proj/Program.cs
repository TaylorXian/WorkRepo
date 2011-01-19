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
			string respFilename = "./header.txt";
			StreamWriter sw = new StreamWriter(File.Create(respFilename));
			string origin = HttpUtility.UrlDecode(TestUrl.URL1);
			sw.WriteLine(origin); 
			sw.WriteLine(TestUrl.URL1);
			sw.WriteLine(HttpUtility.UrlEncode("上帝").ToUpper());
			WebRequest req = WebRequest.Create(TestUrl.URL2);

			HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
			//foreach (string str in resp.Headers)
			//{
			//    File.AppendAllText(respFilename, str);
			//}
			Stream stream = resp.GetResponseStream();
			Console.WriteLine(resp.CharacterSet);
			Console.WriteLine(resp.ContentEncoding);
			Console.WriteLine(resp.ContentLength);
			Console.WriteLine(resp.ContentType);
			StreamReader sr = new StreamReader(stream, Encoding.Default);
			sw.WriteLine(sr.CurrentEncoding);
			sw.WriteLine(sr.ReadToEnd());
			sr.Close();
			sw.Close();
			
			//Console.ReadKey();
		}
	}
}
