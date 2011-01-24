using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.IO;

namespace GoogleTranslateAPI
{
	/// <summary>
	/// Summary description for Trans
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	[System.Web.Script.Services.ScriptService]
	public class Trans : System.Web.Services.WebService
	{

		[WebMethod]
		public string HelloWorld()
		{
			return "Hello World";
		}
		[WebMethod]
		public string GetText(int index)
		{
			return "你好";
		}
		[WebMethod]
		public int RecText(string tran)
		{
			try
			{
				File.AppendAllText("./log.txt", tran);
			}
			catch (Exception)
			{
				return 1;
			}
			return 0;
		}
	}
}
