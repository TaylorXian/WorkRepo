using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HttpWorker;

namespace Translate
{
	public class MyTranslate
	{
		//public static string Translate(string text)
		//{
		//    if (!string.IsNullOrEmpty(text.Trim()))
		//    {
		//        System.Threading.Thread.Sleep(4 * 1000);
		//        Google.API.Translate.TranslateClient client = new Google.API.Translate.TranslateClient(null);
		//        try
		//        {
		//            string translated = client.Translate(text,
		//                Google.API.Translate.Language.ChineseSimplified,
		//                Google.API.Translate.Language.English);
		//            Console.WriteLine(translated);
		//            return translated;

		//        }
		//        catch (Google.API.GoogleAPIException ex)
		//        {
		//            File.AppendAllText("trans.log", ex.Message);
		//            File.AppendAllText("trans.log", "\r\n");
		//            System.Threading.Thread.Sleep(5 * 1000);
		//        }
		//        return null;
		//    }
		//    throw new Exception("the text translated mustn't be empty or null.");
		//}
		public static string Translate(string text)
		{
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				return IcibaTrans.Trans(text);
			}
			throw new Exception("the text translated mustn't be empty or null.");
		}
		public static string TranslateY(string text)
		{
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				return YoudaoTrans.Trans(text);
			}
			throw new Exception("the text translated mustn't be empty or null.");
		}
	}
}
