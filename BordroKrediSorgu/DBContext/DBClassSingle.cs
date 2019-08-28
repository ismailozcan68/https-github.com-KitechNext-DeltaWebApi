using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using System.Configuration;

namespace Delta.UTL.DBUtil
{
    public struct Parameter
    {
        public string ParamName;
        public object ParamValue;
    }

    public class DBClassSingle
    {
        private SqlConnection dbConn;
        public string errorStr = string.Empty;
        public int lastInsertId;
        public static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        SqlCommand cmd = new SqlCommand();

        public DBClassSingle()
        {
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

        public string ConnectionString
        {
            get
            {
                try
                {
                    return EncryptionLibrary.DecryptText(ConfigurationManager.ConnectionStrings["Database"].ConnectionString,EncryptionLibrary.defaultEncryptionKey);
                }
                catch (Exception)
                {
                    return ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
                }
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
                    logger.Error("DBClass.connectDB:" + ex);
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

        #region ExecutionCommands
        public DataSet ExecuteDataSet(string spName, Version version, string language, out string errorMessage, List<Parameter> paramList, long? sessionId = null)
        {
            errorMessage = "";
            DataSet result = new DataSet();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = dbConn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@spName", spName);
                    cmd.Parameters.AddWithValue("@version", version.ToString());
                    cmd.Parameters.AddWithValue("@language", language);
                    if (paramList != null)
                    {
                        //string parameters = StringUtil.SerializeObjectToXML(paramList);
                        //cmd.Parameters.AddWithValue("@parameters", parameters);

                        foreach (Parameter p in paramList)
                        {
                            cmd.Parameters.AddWithValue("@" + p.ParamName, p.ParamValue);
                        }

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
                string msg = ex.Message + Environment.NewLine + "exec " + spName + " " + StringUtil.ParameterListToStr(paramList);
                //writeToLog("ExecuteDataSet Exception:", msg);
                errorStr = ex.Message;
                errorMessage = ex.Message;
            }
            finally
            {
                disconnectDB();
            }
            return result;
        }
        
        public DataTable ExecuteDataTable(string spName, Version version, string language, out string errorMessage, List<Parameter> paramList, long? sessionId = null)
        {
            errorMessage = "";
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = dbConn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = spName;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.AddWithValue("@spName", spName);
                    cmd.Parameters.AddWithValue("@version", version.ToString());
                    cmd.Parameters.AddWithValue("@language", language);
                    if (paramList != null)
                    {
                        //string parameters = StringUtil.SerializeObjectToXML(paramList);
                        //cmd.Parameters.AddWithValue("@parameters", parameters);

                        foreach (Parameter p in paramList)
                        {
                            cmd.Parameters.AddWithValue("@" + p.ParamName, p.ParamValue);
                        }
               
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
                disconnectDB();
            }
            return dt;
        }

        public DataTable ExecuteDataTable(string spName, Version version, string language, out string errorMessage, string paramList = null, long? sessionId = null)
        {
            errorMessage = "";
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
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
                disconnectDB();
            }
            return dt;
        }

        public DataTable ExecuteSql(string sql, out string errorMessage, params Parameter[] parameters)
        {
            errorMessage = "";
            DataTable dt = new DataTable();
            try
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    connectDB();
                    cmd.Connection = dbConn;
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
                disconnectDB();
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
    }
}