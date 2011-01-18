using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using DataEntity;
using System.IO;

namespace HttpWorker
{
	public class HtmlPage
	{
		HtmlDocument htmlDoc;
		string logpath;

		public string Logpath
		{
			set { logpath = value; }
		}
		public HtmlPage(string url)
			: this(url, string.Empty)
		{
		}

		public HtmlPage(string url, string logpath)
		{
			this.logpath = logpath;
			htmlDoc = HttpProc.HttpGetHtmlDocument(url, true);
		}

		public IList<Review> GetReviwList()
		{
			IList<Review> reviews = new List<Review>();
			HtmlNode shopRemark = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"shopRemark\"]");
			HtmlNodeCollection nodes = shopRemark.SelectNodes("//dl[@class='contList']");// and translate(., '&nbsp;', ' ')
			for (int i = 0; i < nodes.Count; i++)
			{
				try
				{
					HtmlNode node = nodes[i];
					Review r = GetReview(node);
					reviews.Add(r);
					File.AppendAllText(logpath, i.ToString() + r.Username);
				}
				catch (Exception ex)
				{
					File.AppendAllText(logpath, ex.Message);
					File.AppendAllText(logpath, "\r\n");
				}
			}
			return reviews;
		}

		private Review GetReview(HtmlNode node)
		{
			Review r = new Review();
			r.Username = GetText(GetSingleDescendantNode(node, "dt/cite/a"));
			r.Content = GetText(GetSingleDescendantNode(node, "dd/div"));
			r.StarLevel = GetSingleDescendantNode(node, "dd/ul/li/span[@class]").Attributes["class"].Value;
			r.StarLevel = GetStarLever(r.StarLevel);
			r.PostTime = GetText(GetSingleDescendantNode(node, "dd/ul[2]/li/span[@class='review-date']"));
			r.PostTime = GetTimeString(r.PostTime);
			Console.WriteLine(r.Username);
			Console.WriteLine(r.PostTime);
			Console.WriteLine(r.StarLevel);
			Console.WriteLine(r.Content);
			return r;
		}

        //private string GetAllNodesText(HtmlNode htmlNode)
        //{
        //    htmlNode.SelectNodes("");
        //}

		private string GetStarLever(string p)
		{
			if (string.IsNullOrEmpty(p))
			{
				return "0";
			}
			for (int i = 0; i < p.Length; i++)
			{
				switch (p[i])
				{
					case '0':
						return "0";
					case '1':
						return "2";
					case '2':
						return "4";
					case '3':
						return "6";
					case '4':
						return "8";
					case '5':
						return "10";
					default:
						break;
				}
			}

			return "0";
		}

		private string GetTimeString(string time)
		{
			StringBuilder t = new StringBuilder(time);
			for (int i = 0; i < t.Length; i++)
			{
				switch (t[i])
				{
					case '0':
					case '1':
					case '2':
					case '3':
					case '4':
					case '5':
					case '6':
					case '7':
					case '8':
					case '9':
					case ':':
					case '-':
					case ' ': break;
					case '更':
					case '新':
					case '于':
						if (i > 0)
						{
							t.Remove(0, i);
							i = 0;
						}
						t.Remove(i--, 1);
						break;
					default:
						t.Remove(i--, 1);
						break;
				}
			}
			if (t.Length > 0 && int.Parse(t[0].ToString()) > 1)
			{
				t.Insert(0, "19");
			}
			else
			{
				t.Insert(0, "20");
			}
			return t.ToString();
		}

		private static HtmlNode GetSingleDescendantNode(HtmlNode node, string xpath)
		{
			return node.SelectSingleNode(string.Format("./descendant::{0}", xpath));
		}

		private static string GetText(HtmlNode node)
		{
			if (node == null)
			{
				return string.Empty;
			}
            string txt = node.InnerText.Replace("&nbsp", " ");// SelectSingleNode("text()").InnerText;
			//Console.WriteLine(txt);
			return txt;
		}
	}
}
