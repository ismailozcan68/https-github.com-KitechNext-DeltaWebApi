using Delta.UTL.DBUtil;
using DeltaWebApi.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace DeltaWebApi.DBContext
{
    public class DBClass
    {
        private SqlConnection dbConn;
        public string errorStr = string.Empty;
        public int lastInsertId;
        private static string respositoryEncryptionKey = "!!Deltasql1920!!@";
        private long _repositoryId;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        SqlCommand cmd = new SqlCommand();

        public DBClass()
        {
            errors = new List<SqlError>(5);
        }

        public DBClass(long RepositoryId)
        {
            _repositoryId = RepositoryId;
            errors = new List<SqlError>(5);
        }

        public bool isConnected
        {
            get
            {
                if (dbConn != null)
                    if (dbConn.State == ConnectionState.Closed)
                        return false;
                    else if (dbConn.State == ConnectionState.Open)
                        return true;
                    else
                        return false;
                else
                    return false;
            }
            set { }
        }

        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            }
        }

        public void connectDB()
        {
            if (!isConnected)
            {
                SqlConnection myConn = new SqlConnection();
                myConn.ConnectionString = ConnectionString;
                try
                {
                    myConn.Open();
                    dbConn = myConn;
                }

                catch (SqlException ex)
                {
                    //writeToLog("connectDB", ex.Message);
                    errorStr = ex.Message;
                }
            }
        }

        protected void disconnectDB()
        {
            if (dbConn != null)
            {
                dbConn.Close();
                dbConn.Dispose();
                dbConn = null;
            }
        }


        #region RepoDB

        private static SortedList<long, string> Repositories = null;

        private SqlConnection repoDbConn;

        private string RepoConnectStr(long RepositoryId)
        {
            string connStr = null;
            if (Repositories == null || !Repositories.Keys.Contains(RepositoryId))
            {
                LoadRepositories();
            }

            if (Repositories != null && Repositories.Keys.Contains(RepositoryId))
            {
                connStr = Repositories[RepositoryId];
            }

            return connStr;
        }

        private void LoadRepositories()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    cmd.Connection = dbConn;
                    cmd.CommandText = "Select Id,RepositoryName,ConnString from Repositories Where IsDeleted=0";
                    cmd.CommandTimeout = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                    Repositories = new SortedList<long, string>();

                    foreach (DataRow item in dt.Rows)
                    {
                        string strConnString = item["ConnString"].ToString();
                        try
                        {
                            strConnString = StringUtil.ConvertFromBase64(item["ConnString"].ToString(), true, respositoryEncryptionKey).ToString();
                        }
                        catch (System.Exception ex)
                        {
                            errorStr = "Can not get repository connection from master db information,check connection string in master db " + ex.Message;
                            //throw new Exception(errorStr);
                        }
                        Repositories.Add(long.Parse(item["Id"].ToString()), strConnString);
                    }
                }
            }
            catch (SqlException ex)
            {
                errorStr = ex.Message;
            }
            finally
            {
                disconnectDB();
            }
        }

        public DataTable LoadUserRepositories(string UserCode)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    cmd.Connection = dbConn;
                    cmd.CommandText = @"Select r.Id, r.RepositoryName from 
                                        GlobalUsers g left join RepositoryUsers ru on g.Id = ru.GlobalUser_Id and ru.IsDeleted = 0  
                                        left join Repositories r on r.Id = ru.Repository_Id and r.IsDeleted = 0
                                        Where g.UserCode = @UserCode and g.IsDeleted = 0";
                    cmd.Parameters.AddWithValue("UserCode", UserCode);

                    cmd.CommandTimeout = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                    return dt;
                }
            }
            catch (SqlException ex)
            {
                errorStr = ex.Message;
            }
            finally
            {
                disconnectDB();
            }

            return dt;
        }

        public DataTable GetAvailableLanguages()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    cmd.Connection = dbConn;
                    cmd.CommandText = @"select Id,Name,Code
		                                from SetupLanguages with(nolock)
		                                order by Name";
                    cmd.CommandTimeout = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                    return dt;
                }
            }
            catch (SqlException ex)
            {
                errorStr = ex.Message;
            }
            finally
            {
                disconnectDB();
            }

            return dt;
        }

        public void connectRepoDB(long RepositoryId)
        {

            string connStr = RepoConnectStr(RepositoryId);
            if (connStr != null)
            {
                SqlConnection myConn = new SqlConnection();
                myConn.ConnectionString = connStr;
                try
                {
                    myConn.Open();
                    repoDbConn = myConn;
                }
                catch (SqlException ex)
                {
                    errorStr = ex.Message;
                }
            }
            else
            {
                errorStr = "Can not get repository db connetion info from master db information,check connection string in master db";
                throw new Exception(errorStr);
            }

            if (repoDbConn == null)
                throw new Exception("Can not get conntection for Repository Id: " + RepositoryId);
        }

        protected void disconnectRepoDB()
        {
            if (repoDbConn != null)
            {
                repoDbConn.Close();
                repoDbConn.Dispose();
                repoDbConn = null;
            }
        }

        public SortedList<long, string> getActiveRepositories(bool isActive = false)
        {

            SortedList<long, string> repos = new SortedList<long, string>();

            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    cmd.Connection = dbConn;
                    cmd.CommandText = "Select Id,RepositoryName,ConnString from Repositories Where IsDeleted=0 and Show=1"; // + isActive.GetInt().ToString();
                    cmd.CommandTimeout = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                    foreach (DataRow item in dt.Rows)
                    {
                        repos.Add(item["Id"].GetLong(), item["RepositoryName"].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                errorStr = ex.Message;
            }
            finally
            {
                disconnectDB();
            }

            return repos;
        }

        public SortedList<long, string> getActiveCompaniesOfRepositories(long repositoryId)
        {
            SortedList<long, string> comp = new SortedList<long, string>();

            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectRepoDB(repositoryId);
                    cmd.Connection = repoDbConn;
                    cmd.CommandText = "Select Id,Name from Companies where IsDeleted=0 and Show=1";
                    cmd.CommandTimeout = 0;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                    foreach (DataRow item in dt.Rows)
                    {
                        comp.Add(item["Id"].GetLong(), item["Name"].ToString());
                    }
                }
            }
            catch (SqlException ex)
            {
                errorStr = ex.Message;
            }
            finally
            {
                disconnectDB();
            }

            return comp;
        }

        public SqlConnection getRepoConnection(long RepositoryId)
        {
            connectRepoDB(RepositoryId);
            return repoDbConn;
        }

        #endregion

        #region ExecutionCommands
        public DataSet ExecuteDataSet(long RepositoryId, string spName, Version version, string language, out string errorMessage, List<DeltaWebApi.Util.Parameter> paramList, long? sessionId = null)
        {
            errorMessage = "";
            DataSet result = new DataSet();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectRepoDB(RepositoryId);
                    cmd.Connection = repoDbConn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    // KITECH Kapattı
                    //cmd.CommandText = spName;
                    cmd.CommandText = "sp_SenderDataSet";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@spName", spName);
                    cmd.Parameters.AddWithValue("@version", version.ToString());
                    cmd.Parameters.AddWithValue("@language", language);
                    if (paramList != null)
                    {
                        string parameters = SerializeObjectToXML(paramList);
                        cmd.Parameters.AddWithValue("@parameters", parameters);

                        // KITECH Kapattı
                        /*
                        foreach (Parameter p in paramList)
                        {
                           
                            cmd.Parameters.AddWithValue("@" + p.ParamName, p.ParamValue);
                        }
                        */
                    }

                    if (sessionId != null)
                        cmd.Parameters.AddWithValue("@sessionId", sessionId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(result);
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                string msg = ex.Message + Environment.NewLine + "exec " + spName + " "; // iso + StringUtil.ParameterListToStr(paramList);
                //writeToLog("ExecuteDataSet Exception:", msg);
                errorStr = ex.Message;
                errorMessage = ex.Message;
            }
            finally
            {
                disconnectRepoDB();
            }
            return result;
        }

        public DataSet ExecuteDataSet(long RepositoryId, string spName, Version version, string language, out string errorMessage, string paramList = null, long? sessionId = null)
        {
            errorMessage = "";
            DataSet result = new DataSet();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectRepoDB(RepositoryId);
                    cmd.Connection = repoDbConn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    // KITECH Kapattı
                    //cmd.CommandText = spName;
                    cmd.CommandText = "sp_SenderDataSet";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@spName", spName);
                    cmd.Parameters.AddWithValue("@version", version.ToString());
                    cmd.Parameters.AddWithValue("@language", language);
                    if (paramList != null)
                    {
                        cmd.Parameters.AddWithValue("@parameters", paramList);

                        // KITECH Kapattı
                        /*
                        foreach (Parameter p in paramList)
                        {
                           
                            cmd.Parameters.AddWithValue("@" + p.ParamName, p.ParamValue);
                        }
                        */
                    }

                    if (sessionId != null)
                        cmd.Parameters.AddWithValue("@sessionId", sessionId);

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(result);
                    }
                }
                return result;
            }

            catch (Exception ex)
            {
                // KITECH Kapattı
                //string msg = ex.Message + Environment.NewLine + "exec " + spName + " " + UTL.StringUtil.ParameterListToStr(paramList);
                //writeToLog("ExecuteDataSet Exception:", msg);
                errorStr = ex.Message;
                errorMessage = ex.Message;
            }
            finally
            {
                disconnectRepoDB();
            }
            return result;
        }

        public string SerializeObjectToXML(object obj)
        {

            var serializer = new XmlSerializer(obj.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var ms = new MemoryStream();
            //the following line omits the xml declaration
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = new UnicodeEncoding(false, false) };
            var writer = XmlWriter.Create(ms, settings);
            serializer.Serialize(writer, obj, ns);
            return Encoding.Unicode.GetString(ms.ToArray());
        }

        public DataTable ExecuteDataTable(long RepositoryId, string spName, Version version, string language, out string errorMessage, List<DeltaWebApi.Util.Parameter> paramList, long? sessionId = null)
        {
            errorMessage = "";
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    cmd.Connection = dbConn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    // KITECH Kapattı
                    //cmd.CommandText = spName;
                    cmd.CommandText = "sp_SenderDataSet";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@spName", spName);
                    cmd.Parameters.AddWithValue("@version", version.ToString());
                    cmd.Parameters.AddWithValue("@language", language);
                    if (paramList != null)
                    {
                        string parameters = SerializeObjectToXML(paramList);
                        cmd.Parameters.AddWithValue("@parameters", parameters);

                        // KITECH Kapattı
                        /*
                        foreach (Parameter p in paramList)
                        {
                           
                            cmd.Parameters.AddWithValue("@" + p.ParamName, p.ParamValue);
                        }
                        */
                    }

                    if (sessionId != null)
                        cmd.Parameters.AddWithValue("@sessionId", sessionId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                string msg = "exec " + spName + " ";
                //writeToLog(" ExecuteDataTable Exception:", ex.Message + Environment.NewLine + UTL.StringUtil.ParameterListToStr(paramList));
                errorStr = ex.Message;
                errorMessage = ex.Message;
            }
            finally
            {
                disconnectRepoDB();
            }
            return dt;
        }

        public DataTable ExecuteDataTable(long RepositoryId, string spName, Version version, string language, out string errorMessage, string paramList = null, long? sessionId = null)
        {
            errorMessage = "";
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectRepoDB(RepositoryId);
                    cmd.Connection = repoDbConn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    // KITECH Kapattı
                    //cmd.CommandText = spName;
                    cmd.CommandText = "sp_SenderDataSet";
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@spName", spName);
                    cmd.Parameters.AddWithValue("@version", version.ToString());
                    cmd.Parameters.AddWithValue("@language", language);
                    if (paramList != null)
                    {
                        cmd.Parameters.AddWithValue("@parameters", paramList);

                        // KITECH Kapattı
                        /*
                        foreach (Parameter p in paramList)
                        {
                           
                            cmd.Parameters.AddWithValue("@" + p.ParamName, p.ParamValue);
                        }
                        */
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                string msg = "exec " + spName + " ";
                //writeToLog(" ExecuteDataTable Exception:", ex.Message + Environment.NewLine + UTL.StringUtil.ParameterListToStr(paramList));
                errorStr = ex.Message;
                errorMessage = ex.Message;
            }
            finally
            {
                disconnectRepoDB();
            }
            return dt;
        }

        public DataTable ExecuteSql(long RepositoryId, string sql, out string errorMessage, params DeltaWebApi.Util.Parameter[] parameters)
        {
            errorMessage = "";
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectRepoDB(RepositoryId);
                    cmd.Connection = repoDbConn;
                    // KITECH Kapattı
                    //cmd.CommandText = spName;
                    cmd.CommandText = sql;
                    cmd.CommandTimeout = 0;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.AddWithValue($"@{parameter.ParamName}", parameter.ParamValue);
                        }
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
                return dt;
            }

            catch (Exception ex)
            {
                string msg = "exec " + sql + " ";
                //writeToLog(" ExecuteDataTable Exception:", ex.Message + Environment.NewLine + UTL.StringUtil.ParameterListToStr(paramList));
                errorStr = ex.Message;
                errorMessage = ex.Message;
            }
            finally
            {
                disconnectRepoDB();
            }
            return dt;
        }

        #endregion

        private List<SqlError> errors;

        public SqlError[] Parse(string sqlText)
        {
            errors.Clear();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    dbConn.FireInfoMessageEventOnUserErrors = true; //when true, the SqlCommand object will not throw an Exception when errors occur
                    dbConn.InfoMessage += new SqlInfoMessageEventHandler(dbConn_InfoMessage);

                    cmd.Connection = dbConn;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "SET PARSEONLY ON";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = sqlText;
                    cmd.ExecuteNonQuery(); //conn_InfoMessage is invoked for every error, e.g. 2 times for 2 errors

                    cmd.CommandText = "SET PARSEONLY OFF";
                    cmd.ExecuteNonQuery();


                    //cmd.CommandText = sqlText;
                    //DataTable tbl = new DataTable();
                    //SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    //adapter.Fill(tbl);
                }
            }

            catch (Exception ex)
            {
                errorStr = ex.Message;
            }
            finally
            {
                disconnectDB();
            }

            return errors.ToArray();
        }

        private void dbConn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            //ensure that all errors are caught
            SqlError[] errorsFound = new SqlError[e.Errors.Count];
            e.Errors.CopyTo(errorsFound, 0);
            errors.AddRange(errorsFound);
        }

        #region dbUpdates

        //TODO Not TESTED and USED RepositoryID=0 verildi.
        public long GetCurrentAppVersion()
        {
            string message;
            long currentDBVersion = -2;

            DataTable dt = ExecuteSql(0, "Select AppDbVersion from SystemAppVersions order by LastChangeDate desc", out message, null);

            if (dt == null || dt.Rows.Count == 0)
            {
                currentDBVersion = -1;
            }
            else
            {
                if (dt.Rows[0]["AppDbVersion"] != DBNull.Value)
                {
                    currentDBVersion = (long)dt.Rows[0]["AppDbVersion"];
                }
            }
            return currentDBVersion;
        }

        private bool UpdateAppVersion(long AppDbVersion)
        {
            string message;

            DataTable dt = ExecuteSql(0, "INSERT INTO [dbo].[SystemAppVersions] ([AppDbVersion],[LastChangeDate]) Values (@AppDbVersion,GetDate())", out message, new DeltaWebApi.Util.Parameter() { ParamName = "AppDbVersion", ParamValue = AppDbVersion });

            return true;
        }

        private bool ExcuteSQLScriptsFile(string scriptFilePath)
        {
            string script = File.ReadAllText(scriptFilePath);

            // split script on GO command
            IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$",
                                     RegexOptions.Multiline | RegexOptions.IgnoreCase);
            try
            {
                connectDB();
                foreach (string commandString in commandStrings)
                {
                    if (commandString.Trim() != "")
                    {
                        using (var command = new SqlCommand(commandString, this.dbConn))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }
                disconnectDB();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }

        }

        public bool CheckDBUpdates(bool populeSampleData = false)
        {
            long currentDBVersion = this.GetCurrentAppVersion();
            //todos
            // check db connection okey
            // check SystemAppVersions is exist if not ask user to continue 
            // add verbose to show running scripts
            // show execptions mesages to user

            //build version mustbe same with actual version to make it easy follow up

            try
            {
                // if 0 init default script
                if (currentDBVersion <= -1)
                {
                    this.ExcuteSQLScriptsFile(AppDomain.CurrentDomain.BaseDirectory + "\\dbscripts\\initialscript.sql");
                    currentDBVersion = 0;
                    this.UpdateAppVersion(currentDBVersion);
                }
                //initial data
                if (currentDBVersion == 0)
                {
                    this.ExcuteSQLScriptsFile(AppDomain.CurrentDomain.BaseDirectory + "\\dbscripts\\initialdata.sql");
                    currentDBVersion = 1000;
                    this.UpdateAppVersion(currentDBVersion);
                }
                //apply sample data
                if (currentDBVersion == 1000 && populeSampleData)
                {
                    this.ExcuteSQLScriptsFile(AppDomain.CurrentDomain.BaseDirectory + "\\dbscripts\\sampledata.sql");
                }
                //apply db updates

                //apply fix 1
                if (currentDBVersion < 1001)
                {
                    this.ExcuteSQLScriptsFile(AppDomain.CurrentDomain.BaseDirectory + "\\dbscripts\\fix1001.sql");
                }
                //apply fix 2
                if (currentDBVersion < 1002)
                {
                    //todos
                }

            }
            catch (Exception exc)
            {

                throw exc;

            }
            return true;
        }

        public string GetConnStrByRepositoryId(long repositoryId)
        {
            return RepoConnectStr(repositoryId);
        }

        #endregion
    }
}