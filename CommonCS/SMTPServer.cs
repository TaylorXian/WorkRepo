using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CommonCS
{
    /// <summary>
    /// Summary description for SMTPServer.
    /// </summary>
    public class SMTPServer
    {
        public SMTPServer()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public static void SetConfigFile(string sCfgFile)
        {
            _sCfgFile = sCfgFile;
        }

        private static string _GetConfigFilePath()
        {
            // It is valid only in a web project?
            return System.Web.HttpContext.Current.Server.MapPath("~/Config/smtp.xml");
        }
        
        private static void _LoadFromFile()
        {
            string sServerCfgPath = (null == _sCfgFile) ? _GetConfigFilePath() : _sCfgFile;

            StreamReader fs = new StreamReader(sServerCfgPath);
            XmlReader reader = new XmlTextReader(fs);
            XmlSerializer serializer = new XmlSerializer(typeof(SMTPServer));
            _svr = (SMTPServer)serializer.Deserialize(reader);

            fs.Close();
        }

        private static void _SaveToFile(DBServer svr)
        {
            string sServerCfgPath = _GetConfigFilePath();
            StreamWriter fs = new StreamWriter(sServerCfgPath);
            XmlWriter writer = new XmlTextWriter(fs);
            XmlSerializer serializer = new XmlSerializer(typeof(SMTPServer));
            serializer.Serialize(writer, svr);

            fs.Close();
        }

        public static SMTPServer GetSMTPServer()
        {
            if (null == _svr)
            {
                _LoadFromFile();
            }

            return _svr;
        }

        [XmlElement("Server")]
        public string               _sServer;

        [XmlElement("User")]
        public string               _sUser;

        [XmlElement("Password")]
        public string               _sPassword;

        [XmlElement("SSL")]
        public bool                 _fSSL;

        private static string _sCfgFile = null;
        private static SMTPServer _svr = null;
    }
}
