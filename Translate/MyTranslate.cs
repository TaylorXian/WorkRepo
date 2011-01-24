using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translate
{
	public class MyTranslate
	{
		private static string Translate(string text)
		{
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				Google.API.Translate.TranslateClient client = new Google.API.Translate.TranslateClient(null);
				string translated = client.Translate(text,
					Google.API.Translate.Language.ChineseSimplified,
					Google.API.Translate.Language.English);
				Console.WriteLine(translated);
				return translated;
			}
			throw new Exception("the text translated mustn't be empty or null.");
		}
	}
}
