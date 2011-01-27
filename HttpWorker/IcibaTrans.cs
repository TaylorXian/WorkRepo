using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpWorker
{
	public class IcibaTrans
	{
		public static string Trans(string content)
		{
			System.Threading.Thread.Sleep(2 * 1000);
			string url = "http://fy.iciba.com/interface.php";
			return HttpProc.Post(url, string.Format("content={0}&t=gb2en", content));
		}
	}
}
