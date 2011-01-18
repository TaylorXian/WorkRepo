using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace CommonCS
{
	/// <summary>
	/// Summary description for Util.
	/// </summary>
	public class Util
	{
        public static bool SendEmail(string sFrom, string sTo, string sSubject, string sBody)
        {
            bool fSucc=true;
            try
            {
                SMTPServer svr = SMTPServer.GetSMTPServer();
                SmtpClient client = new SmtpClient(svr._sServer);
                MailMessage message = new MailMessage(sFrom, sTo);
                MailAddress replytoaddress = new MailAddress(sFrom);
                message.IsBodyHtml = true;
                message.Body = sBody;
                message.Subject = sSubject;
                message.ReplyTo = replytoaddress;

                client.Credentials = CredentialCache.DefaultNetworkCredentials;
                client.Send(message);

                // REVIEW: User credential setting is not implemented.
                /*
                if (svr._sUser.Length > 0)
                {
                    // - smtp.gmail.com use smtp authentication
                    mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
                    mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", svr._sUser);
                    mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", svr._sPassword);
                }

                if (svr._fSSL)
                {
                    // - smtp.gmail.com use port 465 or 587
                    mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", "465");
                    // - smtp.gmail.com use STARTTLS (some call this SSL)
                    mailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");
                }
                 * */
            }
            catch (SmtpException)
            {
                fSucc = false;
            }
            catch (InvalidOperationException)
            {
                fSucc = false;
            }

            return fSucc;
        }
		
		//////////////////////////////////////////////////
		public static string reformQueryString(NameValueCollection nvcQueryString, params string[] sVariables)
		{
			string sNewQueryString = "?";
			bool fIdentical = false;
            
			if (nvcQueryString != null) 
			{
				foreach (string sName in nvcQueryString)
				{
					fIdentical = false;
					for (int i=0; i<sVariables.Length; i++)
					{
						if (sVariables[i].Equals(sName))
						{
							fIdentical = true;
							break;
						}
					}
					if (!fIdentical)
					{
						string sValue = nvcQueryString[sName];

						if (null != sValue && sValue.Length > 0)
						{
							sNewQueryString += sName + "=" + sValue + "&";
						}
					}
				}
			}
			return sNewQueryString.TrimEnd('?','&');
		}

        public static bool IsNullOrBlank(string s) 
        {
            return ((null == s) || (0 == s.Trim().Length));
        }

        ///////////////////////////////////////////////////////////////////////
        public static bool Boolify(int i)
        {
            return (0 != i) ? true : false;
        }
        public static int Deboolify(bool fTrue)
        {
            return (fTrue) ? 1 : 0;
        }

        ///////////////////////////////////////////////////////////////////////
        public class DbgTimer
        {
            public DbgTimer(int iMaxMilliSec) : this()
            {
                _iMaxMilliSec = iMaxMilliSec;
            }
            public DbgTimer()
            {
                _iStart = Environment.TickCount;
            }
            public bool CheckElapsedTime()
            {
                bool fRet;
                int iNow = Environment.TickCount;

                if (iNow >= _iStart)
                {
                    fRet = ((iNow - _iStart) <= _iMaxMilliSec);
                }
                else
                {
                    fRet = ((int.MaxValue - (_iStart - iNow)) <= _iMaxMilliSec);
                }

                return fRet;
            }
            // in millisecs, careful, this wraps after 24 days...
            public int GetElapsedTime()
            {
                // Assume no wrap, since this is for debug
                int iNow = Environment.TickCount;

                // Doesn't cover all cases, we could wrap and then go pass
                //    _iStart again, still, it's true that this shouldn't
                //    be false if used properly
                Debug.Assert(iNow >= _iStart);

                return iNow - _iStart;
            }

            int _iStart;
            int _iMaxMilliSec;
        }

        ///////////////////////////////////////////////////////////////////////
        public static bool DbgShouldBreak()
        {
            return ((Environment.MachineName.ToLower() != "bjbs1" 
                && !Environment.MachineName.ToLower().StartsWith("bjps"))
                || Debugger.IsAttached);
        }

        // Use this to break on debug builds when you want to be notified of a
        // condition that can happen but you want to learn more about
        public static void DbgNotify()
        {
            if (DbgShouldBreak())
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                else
                {
                    Debugger.Launch();
                }
            }
        }

        /*********************************************************************************
         * 
         * Remove Illegal Characters From the String And Convert Letters To UpperCase
         */
        public  static  string  FilterCharacters(string  sInputString) 
        {
            string  sOutputString = "";
            if (!Util.IsNullOrBlank(sInputString)) 
            {
                char[] sCharArray = System.Text.Encoding.Default.GetChars(System.Text.Encoding.GetEncoding("gb2312").GetBytes(sInputString));
                
                foreach(char b in sCharArray) 
                {
                    byte[]  charBytes = System.Text.Encoding.Default.GetBytes(b.ToString());
                    // 0 - 9 : 48 - 57
                    // A - Z : 65 - 90
                    // a - z:  97 - 122
                    // 
                    if ( (b>=48 && b<=57) || (b>=65 && b<=90) || (b>=97 && b<=122) || (b==' ') || charBytes.Length == 2)
                    {
                        if (b>=97 && b<=122) 
                        {
                            sOutputString = sOutputString + b.ToString().ToUpper();
                        } 
                        else 
                        {
                            sOutputString = sOutputString + b;
                        }
                    } 
                    else 
                    {
                        sOutputString = sOutputString + " ";
                    }
                }
            }
            return  sOutputString;
        }

        /*********************************************************************************
         * 
         * Convert Bytes Into Hex String
         */
        private static string _GetHexStrFromBytes(byte[] myBytes) 
        {
            string hexStr = "";
            if (myBytes != null) 
            {
                if (myBytes.Length == 1) 
                {
                    hexStr = String.Format("{0}",myBytes[0].ToString("X"));
                } 
                else if (myBytes.Length == 2) 
                {
                    hexStr = String.Format("{0}{1}",myBytes[0].ToString("X"),myBytes[1].ToString("X"));
                } 
                else if (myBytes.Length == 3) 
                {
                    hexStr = String.Format("{0}{1}{2}",myBytes[0].ToString("X"),myBytes[1].ToString("X"),myBytes[2].ToString("X"));
                }
            } 
            return  hexStr;
        }

        public static string GetHexString(string str) 
        {
            string sRet = "";

            for (int i=0; i < str.Length; i++) 
            {
                char currentchar = str[i];
                byte[] myBytes = System.Text.Encoding.GetEncoding("gb2312").GetBytes(currentchar.ToString());
                string hexStr = _GetHexStrFromBytes(myBytes);

                sRet += hexStr;
            }

            return  sRet;
        }

        public static string GetHexStringForChineseChars(string str)
        {
            string sRet = "";

            for (int i = 0; i < str.Length; i++)
            {
                char currentchar = str[i];

                if (currentchar >= '\u4e00' && currentchar <= '\u9fa5')
                {
                    byte[] myBytes = System.Text.Encoding.GetEncoding("gb2312").GetBytes(currentchar.ToString());
                    string hexStr = _GetHexStrFromBytes(myBytes);
                    sRet += hexStr;
                }
                else
                {
                    sRet += currentchar;
                }
            }

            return sRet;
        }
	}

    public class DynamicQueryBuilder
    {
        public void AddQueryLine(string sLine)
        {
            _sbQueryText.Append(sLine);
        }

        public void AddQueryLine(string sLine, params string[] p)
        {
            _sbQueryText.Append(sLine);
            for (int i=0; i < p.Length; i++)
            {
                AddParameter(p[i]);
            }
        }

        public void AddParameter(string sParam)
        {
            _params.Add(sParam) ;
        } 

        public string[] GetParameters
        {
            get 
            {
                string[] p = new string[_params.Count];
                for (int i=0; i < _params.Count; i++)
                {
                    p[i] = _params[i].ToString();
                }
                return p;
            }
        } 

        public string GetQueryText
        {
            get 
            { 
                Debug.Assert(_cParamCount == _params.Count, "Params don't match");
                return _sbQueryText.ToString(); 
            }
        }

        public string GetNextParamNumber()
        {
            int iRet = _cParamCount;
            _cParamCount ++;
            return "{" + iRet.ToString() + "}";
        }

        private StringBuilder _sbQueryText = new StringBuilder();
        private ArrayList _params = new ArrayList();
        private int _cParamCount = 0;
    }
}
