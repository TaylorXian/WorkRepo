using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Threading;

namespace CommonCS
{
	/// <summary>
	/// Summary description for ServerFarm.
	/// </summary>
	public class ServerFarm
	{
        public ServerFarm() {}

        public static void SetConfigFile(string sCfgFile)
        {
            _sCfgFile = sCfgFile;
        }

        private static string _GetConfigFilePath()
        {
            // It is valid only in a web project?
            return System.Web.HttpContext.Current.Server.MapPath("~/Config/server.xml");
        }
        
        private static void _LoadFromFile()
        {
            string sServerCfgPath = (null == _sCfgFile) ? _GetConfigFilePath() : _sCfgFile;

            StreamReader fs = new StreamReader(sServerCfgPath);
            XmlReader reader = new XmlTextReader(fs);
            XmlSerializer serializer = new XmlSerializer(typeof(ServerFarm));
            _svrFarm = (ServerFarm)serializer.Deserialize(reader);

            fs.Close();
        }

        private static void _SaveToFile(DBServer svr)
        {
            string sServerCfgPath = _GetConfigFilePath();
            StreamWriter fs = new StreamWriter(sServerCfgPath);
            XmlWriter writer = new XmlTextWriter(fs);
            XmlSerializer serializer = new XmlSerializer(typeof(ServerFarm));
            serializer.Serialize(writer, svr);

            fs.Close();
        }

        public DBServer this[DBRoles role]
        {
            get
            {
                DBServer server = null;

                for (int i = 0; i < _alServerMember.Count; ++i)
                {
                    if (((DBServer)_alServerMember[i])._sDBRole == role.ToString())
                    {
                        server = (DBServer)_alServerMember[i];
                        break;
                    }
                }

                return server;
            }
        }

        public static ServerFarm GetServerFarm()
        {
            if (null == _svrFarm)
            {
                _LoadFromFile();
            }

            return _svrFarm;
        }

        public static DBServer GetComShopServer()
        {
            DBServer svr = (DBServer)_svrFarm._alComShopServers[0];
            for (int i = 1; i < _svrFarm._alComShopServers.Count; i++)
            {
                DBServer server = (DBServer)_svrFarm._alComShopServers[i];

                // if svr._cRunningThreads > server._cRunningThreads
                int minvalue = svr._cRunningThreads;
                Interlocked.Add(ref minvalue, -server._cRunningThreads);
                if (minvalue > 0)
                {
                    svr = server;
                }
            }

            return svr;
        }

        public static DBServer GetUserServer()
        {
            if (null == _svrUser)
            {
                _svrUser = _svrFarm[DBRoles.User];
            }

            return _svrUser;
        }

        public static DBServer GetArticleServer()
        {
            if (null == _svrArticle)
            {
                _svrArticle = _svrFarm[DBRoles.Article];
            }

            return _svrArticle; 
        }

        public static DBServer GetImageServer()
        {
            if (null == _svrImage)
            {
                _svrImage = _svrFarm[DBRoles.Image];
            }

            return _svrImage;
        }

        public static DBServer GetRobotServer()
        {
            if (null == _svrRobot)
            {
                _svrRobot = _svrFarm[DBRoles.Robot];
            }

            return _svrRobot;
        }

        public static DBServer GetUserGenerateServer()
        {
            if (null == _svrUserGenerate)
            {
                _svrUserGenerate = _svrFarm[DBRoles.UserGenerate];
            }

            return _svrUserGenerate;
        }

        public static DBServer GetSpaceServer()
        {
            if (null == _svrSpace)
            {
                _svrSpace = _svrFarm[DBRoles.Space];
            }

            return _svrSpace;
        }

        public static DBServer GetProductionServer()
        {
            if (null == _svrProdution)
            {
                _svrProdution = _svrFarm[DBRoles.Production];
            }

            return _svrProdution;
        }

        public static DBServer GetManualHistoryServer()
        {
            if (null == _svrManualHistory)
            {
                _svrManualHistory = _svrFarm[DBRoles.ManualHistory];
            }
            return _svrManualHistory;
        }

        [XmlArray("ServerMembers")]
        [XmlArrayItem("ServerMember", typeof(DBServer))]
        public ArrayList _alServerMember = new ArrayList();

        [XmlArray("ComShopServerMembers")]
        [XmlArrayItem("ServerMember", typeof(DBServer))]
        public ArrayList _alComShopServers = new ArrayList();

        private static string _sCfgFile = null;
        private static ServerFarm _svrFarm = null;
        private static DBServer _svrUser = null;
        private static DBServer _svrArticle = null;
        private static DBServer _svrImage = null;
        private static DBServer _svrRobot = null;
        private static DBServer _svrUserGenerate = null;
        private static DBServer _svrSpace = null;
        private static DBServer _svrProdution = null;
        private static DBServer _svrManualHistory = null;
	}
}
