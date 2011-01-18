using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CommonCS
{
	/// <summary>
	/// Summary description for DBServer.
	/// </summary>
	public class DBServer
	{
		public DBServer()
		{
            //_sServer = "home";
            //_sUser = "npf";
            //_sPassword = "npf001";
            //_SaveToFile(this);
		}

        [XmlElement("DBRole")]
        public string               _sDBRole;

        [XmlElement("Server")]
        public string               _sServer;

        [XmlElement("User")]
        public string               _sUser;

        [XmlElement("Password")]
        public string               _sPassword;

        [XmlElement("Database")]
        public string               _sDatabase;

        [XmlElement("NumRunningThreads")]
        public int                  _cRunningThreads;
	}
}
