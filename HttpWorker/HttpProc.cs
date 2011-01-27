using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Runtime.Serialization.Json;

namespace HttpWorker
{
	public class HttpProc
	{
		public static void HttpGetFile(string url, string localFilename)
		{
			WebClient request = new WebClient();
			request.DownloadFile(url, localFilename);
		}

		public static HtmlDocument HttpGetHtmlDocument(string url, bool fUTF8)
		{
			HtmlDocument doc = new HtmlDocument();
			doc.OptionOutputAsXml = true;
			using (Stream stream = MyResponseStream(url))
			{
				if (fUTF8)
				{
					doc.Load(stream, Encoding.UTF8);
				}
				else
				{
					doc.Load(stream, Encoding.Default);
				}
				stream.Close();
			}

			return doc;
		}

		public static Stream MyResponseStream(string url)
		{
			return MyResponse(url).GetResponseStream();
		}
		public static WebResponse MyResponse(string url)
		{
			return WebRequest.Create(url).GetResponse();
		}

		private static string XMLHttpRequestPost(string url, string data, string refer)
		{
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.Method = "POST";
			request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 GTB7.1";
			request.Accept = "*/*";
			request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			request.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
			request.Headers.Add("X-Requested-With", "XMLHttpRequest");
			request.Referer = refer;
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);
			request.ContentLength = dataBytes.Length;
			Stream rs = request.GetRequestStream();
			rs.Write(dataBytes, 0, dataBytes.Length);
			rs.Close();
			HttpWebResponse res = request.GetResponse() as HttpWebResponse;
			string restr = null;
			using (StreamReader sr = new StreamReader(res.GetResponseStream()))
			{
				restr = sr.ReadToEnd();
				sr.Close();
				Console.WriteLine(restr.TrimEnd('\n'));
			}
			return restr;
		}

		internal static string Post(string url, string data)
		{
			return XMLHttpRequestPost(url, data, "http://fy.iciba.com/");
		}
		internal static string PostYoudao(string url, string content)
		{
			return XMLHttpRequestPost(url, content, "http://fanyi.youdao.com/");
		}

		internal static string Get(string url, string refer)
		{
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; zh-CN; rv:1.9.2.13) Gecko/20101203 Firefox/3.6.13 GTB7.1";
			request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
			request.Headers.Add("Accept-Language", "zh-cn,zh;q=0.5");
			request.Headers.Add("Accept-Charset", "GB2312,utf-8;q=0.7,*;q=0.7");
			request.Referer = refer;
			HttpWebResponse res = request.GetResponse() as HttpWebResponse;
			string restr = null;
			using (StreamReader sr = new StreamReader(res.GetResponseStream()))
			{
				restr = sr.ReadToEnd();
				sr.Close();
				//Console.WriteLine(restr.TrimEnd('\n'));
			}
			return restr;
		}
	}
}
