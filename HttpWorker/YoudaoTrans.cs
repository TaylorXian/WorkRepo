using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HttpWorker
{
	public class YoudaoTrans
	{
		public static string Trans(string content)
		{
			if (content.Length < 288)
			{
				System.Threading.Thread.Sleep(3 * 1000);
				//string url = "http://fanyi.youdao.com/translate?smartresult=dict&smartresult=rule&sessionFrom=null";

				////"%C3%A9%C2%9B%C2%85%C3%A7%C2%A7%C2%80%C3%A6%C2%9C%C2%8D%C3%A8%C2%A3%C2%85%C3%A5%C2%B8%C2%82%C3%A5%C2%9C%C2%BA"
				// "%E9%A3%9E%E6%9C%BA%E4%BB%80%E4%B9%88%E6%97%B6%E5%80%99%E8%B5%B7%E9%A3%9E"
				//string xml = HttpProc.PostYoudao(url, 
				//    string.Format("type=ZH_CN2EN&i={0}&doctype=xml&xmlVersion=1.4&keyfrom=fanyi.web&ue=UTF-8&flag=false", 
				string enContent = HttpUtility.UrlEncode(content).ToUpper();
				string url = string.Format("http://fanyi.youdao.com/translate?type=ZH_CN2EN&i={0}&keyfrom=dict.top", enContent);
				string refer = string.Format("http://dict.youdao.com/search?q={0}&ue=utf8&keyfrom=dict.index", enContent);
				string html = HttpProc.Get(url, refer);
				return ExtractTrans(html);
			}
			else
			{
				return string.Empty;
			}
		}

		private static string ExtractTrans(string html)
		{
			int start = html.IndexOf("<tgt>");
			int end = html.IndexOf("</tgt>");
			return html.Substring(start, end - start).Replace("<tgt><![CDATA[", "").Replace("]]>", "");
		}
	}
}
