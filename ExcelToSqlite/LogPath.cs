using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ExcelToSqlite
{
	public class LogPath
	{
		public static string GetLogfilepath(string currentDir)
		{
			return Path.GetFullPath(Path.Combine(currentDir, string.Format("./{0}.log", DateTime.Now.ToString("yyyyMMdd HHmmss"))));
		}
	}
}
