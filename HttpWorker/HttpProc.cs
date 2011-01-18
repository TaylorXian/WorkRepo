using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using HtmlAgilityPack;

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
			using (Stream stream = WebRequest.Create(url).GetResponse().GetResponseStream())
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
	}
}
