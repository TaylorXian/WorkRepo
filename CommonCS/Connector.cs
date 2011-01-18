using System;
using System.Data;
using System.Data.Odbc;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace CommonCS
{
    public enum DBRoles
    {
        ComShop = 0,
        User,
        Article,
        Image,
        Robot,
        UserGenerate,
        Space,
        Production,
        ManualHistory,
        _Count_, // Keep last that's the count of items in this list
        _Invalid_,
    }

    public class ConnectionFlags
    {
        public const int None = 0x00000000;
        public const int Read = 0x00000001;
        public const int Write = 0x00000002;
        public const int NullOK = 0x00000004;
        public const int ZeroRecordOK = 0x00000008;
        public const int NoCharEscape = 0x00000010;

        public const int _MaskOper_ = 0x00000003; // Either READ or WRITE

        public const int DbgQueryTimeExempt = 0x10000000;
        public const int DbgQueryRulesExempt = 0x20000000;

    }

    class DBConnection
    {
        public DBConnection(OdbcConnection conn)
        {
            _conn = conn;
        }

        public void Close()
        {
            RealClose();
        }

        // Public because it is accessed from ConnectionStack
        public void RealClose()
        {
            _conn.Close();
        }

        public OdbcConnection Connection
        {
            get { return _conn; }
        }

        private OdbcConnection _conn;
    }

    class ServerLoadUpdater
    {
        private readonly ServerFarm _svrFarm;

        public ServerLoadUpdater(ServerFarm svrFarm)
        {
            _svrFarm = svrFarm;
        }

        private void GetDBRunningThreads(DBServer server)
        {
            OdbcConnection conn = null;

            try
            {
                conn = DataConnection.CreateConnectionToServer(server);
                DataTable dt = new DataTable();

                OdbcDataAdapter da = new OdbcDataAdapter("SHOW GLOBAL STATUS;", conn);

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row[0].ToString() == "Threads_running")
                        {
                            Interlocked.Exchange(ref server._cRunningThreads, Convert.ToInt32(row[1]));
                            break;
                        }
                    }
                }
                else
                {
                    // Shouldn't happen
                    server._cRunningThreads = 50;
                }
            }
            catch (Exception)
            {
                // REVIEW: Out of memory? Made the server look really busy.
                Interlocked.Exchange(ref server._cRunningThreads, Int32.MaxValue);
            }
            finally
            {
                if (null != conn)
                {
                    conn.Close();
                }
            }
        }

        public void UpdateServersLoad()
        {
            if (_svrFarm._alComShopServers.Count > 1)
            {
                while (true)
                {
                    for (int i = 0; i < _svrFarm._alComShopServers.Count; i++)
                    {
                        DBServer server = (DBServer)_svrFarm._alComShopServers[i];
                        // For now use # running threads as the indicator of server load
                        GetDBRunningThreads(server);
                    }

                    Thread.Sleep(5000);
                }
            }
        }
    }

    public class DataConnection
    {
        public enum DBErrors
        {
            None = 0,
            Unknown,
            DuplicateRecord,
            _Count_
        }

        class DbgFlags
        {
            public const int None = 0x00000000;
            public const int AssertKeyUsage = 0x00000001;
            public const int AssertMaxQueryTime = 0x00000002;
            public const int AssertQueryRules = 0x00000004;
        }

        static int s_dbgflags = DbgFlags.None;// | DbgFlags.AssertMaxQueryTime;
        static int s_dbgiMaxQueryTime = 500;

        static DataConnection()
        {
            ServerFarm.GetServerFarm();
            ServerLoadUpdater loadupdater = new ServerLoadUpdater(ServerFarm.GetServerFarm());
            Thread t = new Thread(new ThreadStart(loadupdater.UpdateServersLoad));
            t.Start();
        }

        private DBServer GetServerForRole(DBRoles role)
        {
            DBServer svr = null;

            switch (role)
            {
                case DBRoles.ComShop:
                    svr = ServerFarm.GetComShopServer();
                    break;
                case DBRoles.User:
                    svr = ServerFarm.GetUserServer();
                    break;
                case DBRoles.Article:
                    svr = ServerFarm.GetArticleServer();
                    break;
                case DBRoles.Image:
                    svr = ServerFarm.GetImageServer();
                    break;
                case DBRoles.Robot:
                    svr = ServerFarm.GetRobotServer();
                    break;
                case DBRoles.UserGenerate:
                    svr = ServerFarm.GetUserGenerateServer();
                    break;
                case DBRoles.Space:
                    svr = ServerFarm.GetSpaceServer();
                    break;
                case DBRoles.Production:
                    svr = ServerFarm.GetProductionServer();
                    break;
                case DBRoles.ManualHistory:
                    svr = ServerFarm.GetManualHistoryServer();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return svr;
        }

        public static OdbcConnection CreateConnectionToServer(DBServer svr)
        {
            OdbcConnection odbcconn = new OdbcConnection("DRIVER={MySQL ODBC 3.51 Driver};" +
               "Server=" + svr._sServer + ";" +
               "Port=3306;" +
               "Option=16384;" +
               "Pooling=true;" +
               "Stmt=;" +
               "Database=" + svr._sDatabase + ";" +
               "Uid=" + svr._sUser + ";" +
               "Pwd=" + svr._sPassword + ";");

            odbcconn.Open();

            return odbcconn;
        }

        private OdbcConnection _CreateConnectionWorker(DBRoles role)
        {
            // Image server is not using MySQL
            Debug.Assert(DBRoles.Image != role);

            _svr = GetServerForRole(role);

            OdbcConnection odbcconn = CreateConnectionToServer(_svr);

            //The following steps are due to our DB is using GBK encoding 
            OdbcCommand cmd = new OdbcCommand("SET NAMES 'gbk';", odbcconn);

            cmd.ExecuteNonQuery();

            return odbcconn;
        }

        private DBConnection _GetConnector(DBRoles role)
        {
            OdbcConnection odbcconn = _CreateConnectionWorker(role);
            DBConnection conn = new DBConnection(odbcconn);

            return conn;
        }

        private string EscapeSpecialChars(string s)
        {
            // We don't escape '\'', so make sure that the string is quoted with '\"'
            StringBuilder sb = new StringBuilder((int)(s.Length * 1.25));

            for (int i = 0; i < s.Length; ++i)
            {
                switch (s[i])
                {
                    case '\0':
                        sb.Append("\\0");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '"':
                        sb.Append("\\\"");
                        break;
                    default:
                        sb.Append(s[i]);
                        break;
                }
            }

            return sb.ToString();
        }

        // BUGBUG: This should be const...
        private string[] s_rgsDBNames = new string[]
        {
            "COMSHOP.",
        };

        private void DbgAssertQueryRules(string sQueryTemplate, string sQuery)
        {
            if (Util.Boolify(DbgFlags.AssertQueryRules & s_dbgflags))
            {
                string sQueryTemplateUppercase = sQueryTemplate.ToUpper();

                for (int i = 0; i < s_rgsDBNames.Length; ++i)
                {
                    Debug.Assert(-1 == sQueryTemplateUppercase.IndexOf(s_rgsDBNames[i]));
                }
            }
        }

        private void DbgAssertKeyUsage(string sQuery, OdbcConnection conn)
        {
            //          This doesn't work for now.  We're getting back the actual data,
            //          not the EXPLAIN results...

            if (Util.Boolify(DbgFlags.AssertKeyUsage & s_dbgflags))
            {
                string sExplain = "EXPLAIN " + sQuery;

                DataTable dt = new DataTable();
                OdbcDataAdapter da = new OdbcDataAdapter(sExplain, conn);
                da.Fill(dt);

                // We should get at least one row back, unless the query is
                // malformed which still want to know about
                Debug.Assert(dt.Rows.Count > 0);

                foreach (DataRow dr in dt.Rows)
                {
                    object obj = dr["key"];
                    Debug.Assert((System.DBNull.Value != obj) && (null != obj));
                }
            }
        }

        private string GetQueryStringFromStringParams(int flags, string sQueryTemplate, string[] rgstr)
        {
            string[] rgstrEscaped;

            if (!Util.Boolify(ConnectionFlags.NoCharEscape & flags))
            {
                rgstrEscaped = new string[rgstr.Length];

                for (int i = 0; i < rgstr.Length; ++i)
                {
                    rgstrEscaped[i] = EscapeSpecialChars(rgstr[i]);
                }
            }
            else
            {
                rgstrEscaped = rgstr;
            }

            return string.Format(sQueryTemplate, rgstrEscaped);
        }

        public DBErrors GetLastError()
        {
            Debug.Assert(DBErrors.None != _err);
            return _err;
        }

        private delegate bool _DelegateQuery(string sQuery, OdbcConnection conn,
            int flags, out Object obj);

        private bool _QueryWorker(_DelegateQuery delegatequery, out Object obj,
            DBRoles role, int flags, string sQueryTemplate,
            params string[] rgstr)
        {
            bool fRet = false;
            string sQuery = null;
            DBConnection conn = null;

            Debug.Assert(Util.Boolify(ConnectionFlags._MaskOper_ & flags));
            Debug.Assert((ConnectionFlags._MaskOper_ & flags) != ConnectionFlags._MaskOper_);

            // We might want to split this try/catch into two: get connection
            // and deletegatequery

            try
            {
                conn = _GetConnector(role);

                Util.DbgTimer timer;

                sQuery = GetQueryStringFromStringParams(flags, sQueryTemplate, rgstr);

                DbgAssertKeyUsage(sQuery, conn.Connection);
                DbgAssertQueryRules(sQueryTemplate, sQuery);

                if (Util.Boolify(DbgFlags.AssertMaxQueryTime & s_dbgflags))
                {
                    timer = new Util.DbgTimer(s_dbgiMaxQueryTime);
                }
                else
                {
                    timer = null;
                }

                _err = DBErrors.None;

                fRet = delegatequery(sQuery, conn.Connection, flags, out obj);

                if (Util.Boolify(DbgFlags.AssertMaxQueryTime & s_dbgflags))
                {
                    Debug.Assert(timer.CheckElapsedTime());
                }
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf("Duplicate") > 0)
                {
                    _err = DBErrors.DuplicateRecord;
                }
                else
                {
                    _err = DBErrors.Unknown;
                    Util.DbgNotify();
                    Error.LogEvent(Error.Severities.Error, "Exception \"{0}\"\r\n    for query: \"{1}\"",
                        e.Message, sQuery);
                }

                fRet = false;
                obj = null;
            }
            finally
            {
                if (null != conn)
                {
                    // BUGBUG: Can probably also throw, throwing is fun...
                    conn.Close();
                }
            }

            return fRet;
        }

        private bool _GetDataTableWorker(string sQuery, OdbcConnection conn,
            int flags, out Object obj)
        {
            bool fRet;
            DataTable dt = new DataTable();

            OdbcDataAdapter da = new OdbcDataAdapter(sQuery, conn);

            da.Fill(dt);

            if (dt.Rows.Count > 0 || Util.Boolify((ConnectionFlags.ZeroRecordOK & flags)))
            {
                fRet = true;
            }
            else
            {
                fRet = false;
                Error.LogEvent(Error.Severities.Warning, "NULL dt for: \"{0}\"", sQuery);
            }

            obj = dt;

            return fRet;
        }

        public bool GetDataTable(DBRoles role, out DataTable dt,
            string sQueryTemplate, params string[] rgstr)
        {
            object obj;

            bool fRet = _QueryWorker(new _DelegateQuery(_GetDataTableWorker),
                out obj, role, ConnectionFlags.Read, sQueryTemplate, rgstr);

            if (fRet)
            {
                dt = (DataTable)obj;
            }
            else
            {
                dt = null;
            }

            return fRet;
        }

        public bool GetDataTableEx(DBRoles role, int flags, out DataTable dt,
            string sQueryTemplate, params string[] rgstr)
        {
            object obj;

            bool fRet = _QueryWorker(new _DelegateQuery(_GetDataTableWorker),
                out obj, role, flags, sQueryTemplate, rgstr);

            if (fRet)
            {
                dt = (DataTable)obj;
            }
            else
            {
                dt = null;
            }

            return fRet;
        }

        private bool _QueryScalarWorker(string sQuery, OdbcConnection conn,
            int flags, out Object obj)
        {
            bool fRet;

            // Setup the ODBC command
            OdbcCommand cmd = new OdbcCommand(sQuery, conn);

            // Execute the query
            obj = cmd.ExecuteScalar();

            if (null != obj || Util.Boolify(ConnectionFlags.NullOK & flags))
            {
                fRet = true;
            }
            else
            {
                fRet = false;
                Error.LogEvent(Error.Severities.Warning, "NULL dt for: \"{0}\"", sQuery);
            }

            return fRet;
        }

        public bool QueryScalar(DBRoles role, int flags,
            out Object obj, string sQueryTemplate, params string[] rgstr)
        {
            Debug.Assert(Util.Boolify(ConnectionFlags.Read & flags));

            bool fRet = _QueryWorker(new _DelegateQuery(_QueryScalarWorker), out obj,
                role, flags, sQueryTemplate, rgstr);

            return fRet;
        }

        public bool QueryScalarAsInt(DBRoles role, out int i,
            string sQueryTemplate, params string[] rgstr)
        {
            Object obj;
            bool fRet = _QueryWorker(new _DelegateQuery(_QueryScalarWorker),
                out obj, role, ConnectionFlags.Read,
                sQueryTemplate, rgstr);

            if (fRet)
            {
                if (obj is System.DBNull)
                {
                    i = -1;
                    fRet = false;
                }
                else
                {
                    i = Int32.Parse(obj.ToString());
                }
            }
            else
            {
                // To make compiler happy and people who don't check return
                // values unhappy
                i = -1;
            }

            return fRet;
        }

        public bool QueryScalarAsString(DBRoles role, out string s,
            string sQueryTemplate, params string[] rgstr)
        {
            Object obj;
            bool fRet = _QueryWorker(new _DelegateQuery(_QueryScalarWorker),
                out obj, role, ConnectionFlags.Read,
                sQueryTemplate, rgstr);

            if (fRet)
            {
                if (obj is System.DBNull)
                {
                    s = null;
                    fRet = false;
                }
                else
                {
                    s = obj.ToString();
                }
            }
            else
            {
                // To make compiler happy and people who don't check return
                // values unhappy
                s = null;
            }

            return fRet;
        }

        private bool _ExecuteNonQueryWorker(string sQuery, OdbcConnection conn,
            int flags, out Object obj)
        {
            bool fRet;

            // Setup the ODBC command
            OdbcCommand cmd = new OdbcCommand(sQuery, conn);

            // Execute the query
            int cAffectedRows = cmd.ExecuteNonQuery();

            if ((0 != cAffectedRows) || Util.Boolify(ConnectionFlags.ZeroRecordOK & flags))
            {
                fRet = true;
            }
            else
            {
                fRet = false;
            }

            obj = cAffectedRows;

            return fRet;
        }

        public bool ExecuteNonQuery(DBRoles role, string sQueryTemplate, params string[] rgstr)
        {
            Object obj;

            bool fRet = _QueryWorker(new _DelegateQuery(_ExecuteNonQueryWorker),
                out obj, role, ConnectionFlags.Write, sQueryTemplate, rgstr);

            return fRet;
        }

        public bool ExecuteNonQueryEx(DBRoles role, int flags, out int cRowsAffected,
            string sQueryTemplate, params string[] rgstr)
        {
            Debug.Assert(Util.Boolify(ConnectionFlags.Write & flags));

            Object obj;

            bool fRet = _QueryWorker(new _DelegateQuery(_ExecuteNonQueryWorker),
                out obj, role, flags, sQueryTemplate, rgstr);

            if (fRet)
            {
                cRowsAffected = (int)obj;
            }
            else
            {
                cRowsAffected = 0;
            }

            return fRet;
        }

        private static DBServer _svr = null;

        private DBErrors _err;
    }
}
