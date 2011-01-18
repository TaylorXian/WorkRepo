using System;
using System.IO;
using System.Threading;

namespace CommonCS
{
	/// <summary>
	/// Summary description for Error.
	/// </summary>
	public class Error
	{
        public enum Severities
        {
            CriticalError,  // Something that indicates that one of our systems is non-operational or
            // becoming non-operational.  Also for major software bug that would
            // compromise system operation.
            // When one of these will be logged, a pager will probably ring somewhere.
            // E.g.: Trying to perform a query against a DBServer that should be
            //       operational and get a failure.

            Error,          // Something that should not happen happenned but that won't prevent the
            // system from being operational.
            // These should be reviewed periodically (e.g.: daily) but do not constitute
            // emergencies.
            // E.g.: A request for a specific ForumGUID is made and fails.  Either we
            //       have data inconsistency in one or more DBs, data corruption or 
            //       a user is trying to access our data outside of our client.

            Warning,        // An operation that failed because it goes outside of the system's normal
            // operation.
            // Should be reviewed periodically for trends and potential improvements to
            // the system.
            // E.g.: Failed user creation, failed login attempts.  Searches that return
            //       no results.  System that passes some load threshold.
                
            Debug,          // Operation logging.
            // For diagnostic purposes.
            // E.g.: When a system starts, is deliberately taken down, ...
        };

        private static Mutex s_Mutex = new Mutex();
        
        static public void LogEvent(Severities severity, string sTemplate, params string[] rgstr)
        {
            if (s_Mutex.WaitOne())
            {
                string sMessage = string.Format(sTemplate, rgstr);

                _writer.WriteLine("=======================");
                _writer.WriteLine("Message:  \"{0}\"", sMessage);
                _writer.WriteLine("Severity: {0}\r\nTime: {1}", severity.ToString(), DateTime.Now.ToString());
                _writer.WriteLine("StackTrace:\r\n{0}", Environment.StackTrace);
                _writer.Flush();
                s_Mutex.ReleaseMutex();
            }
        }

        static Error()
        {
            string strExpanded;
            
            if (null != System.Web.HttpContext.Current)
            {
                strExpanded = System.Web.HttpContext.Current.Server.MapPath("~/Config/logs/log_");
            }
            else
            {
                strExpanded = @"c:\robot\logs\log_";
            }
            
            strExpanded += DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt";
            _fs = new FileStream(strExpanded, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            _writer = new StreamWriter(_fs);
        }

        static FileStream _fs;
        static StreamWriter _writer;
	}
}
