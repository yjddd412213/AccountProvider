using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace CommonLib
{
    public class DataAccess
    {
        #region vars

        /// <summary>
        /// err msg
        /// </summary>
        private string strErr = "";

        /// <summary>
        /// SqlConnection
        /// </summary>
        private SqlConnection objConnection;

        #endregion

        #region instance

        /// <summary>
        /// conn
        /// </summary>
        public DataAccess()
        {
            try
            {
                //objConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyDbConn1"].ToString());
                //objConnection = new SqlConnection("Server=tcp:pkrli5hvlc.database.windows.net,1433;Database=InywhereCompany;User ID=clarence@pkrli5hvlc;Password=Liu890405Lei;Trusted_Connection=False;Encrypt=True;");
                objConnection = new SqlConnection("Data Source=192.168.1.150;Initial Catalog=InywherePaymentInfo ;uid=inywhere;pwd=inywhere;");
            }
            catch (Exception objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
                throw new Exception(strErr);
            }
        }

        #endregion

        #region property

        /// <summary>
        /// err
        /// </summary>
        public string ErrMessage
        {
            get
            {
                return strErr;
            }
        }

        /// <summary>
        /// get conn
        /// </summary>
        public SqlConnection dbConnection
        {
            get
            {
                return objConnection;
            }
        }

        #endregion

        #region open and close

        /// <summary>
        /// open
        /// </summary>
        private void OpenConnection()
        {
            try
            {
                if (objConnection.State != ConnectionState.Open)
                {
                    objConnection.Open();
                }
            }
            catch (Exception objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
        }

        /// <summary>
        /// close
        /// </summary>
        private void CloseConnection()
        {
            try
            {
                if (objConnection.State == ConnectionState.Open)
                {
                    objConnection.Close();
                }
            }
            catch (Exception objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
        }

        #endregion

        #region execute

        /// <summary>
        ///	get a DataSet by sql
        /// </summary>
        /// <param name="strSql">the sql</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string strSql)
        {
            this.OpenConnection();
            DataSet objDataSet = new DataSet();
            try
            {
                SqlDataAdapter objAdapter = new SqlDataAdapter(strSql, objConnection);
                objAdapter.Fill(objDataSet);
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
            return objDataSet;
        }
        /// <summary>
        /// get a DataReader by sql
        /// </summary>
        /// <param name="vSQLStatement">the sql</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader GetDataReader(string strSql)
        {
            this.OpenConnection();
            SqlDataReader objDataReader = null;
            try
            {
                SqlCommand objCommand = new SqlCommand(strSql, objConnection);
                objDataReader = objCommand.ExecuteReader();
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
                this.CloseConnection();
            }
            return objDataReader;
        }
        /// <summary>
        ///	get a DataTable by sql
        /// </summary>
        /// <param name="strSql">the sql</param>
        /// <returns>DataSet</returns>
        public DataTable GetDataTable(string strSql)
        {
            this.OpenConnection();
            DataTable objDt = new DataTable();
            try
            {
                SqlDataAdapter objAdapter = new SqlDataAdapter(strSql, objConnection);
                objAdapter.Fill(objDt);
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
            return objDt;
        }
        /// <summary>
        /// exec sal
        /// </summary>
        /// <param name="strSql">the sql</param>
        public void ExecuteNonQuery(string strSql)
        {
            this.OpenConnection();
            try
            {
                SqlCommand objCommand = new SqlCommand(strSql, objConnection);
                objCommand.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
        }
        /// <summary>
        /// exec sal
        /// </summary>
        /// <param name="strSql">the sql</param>
        public string ExecuteScalar(string strSql)
        {
            this.OpenConnection();
            try
            {
                SqlCommand objCommand = new SqlCommand(strSql, objConnection);
                object o = objCommand.ExecuteScalar();
                if (o == null)
                    return "";
                else
                    return o.ToString();
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
            return "";
        }


        /// <summary>
        ///	get DataSet by sql
        /// </summary>
        /// <param name="strSql">sql</param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string strSql, SqlCommand comm)
        {
            this.OpenConnection();
            DataSet objDataSet = new DataSet();
            try
            {
                comm.CommandText = strSql;
                comm.Connection = objConnection;
                SqlDataAdapter objAdapter = new SqlDataAdapter(comm);

                objAdapter.Fill(objDataSet);
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
            return objDataSet;
        }
        /// <summary>
        /// get DataReader by command
        /// </summary>
        /// <param name="vSQLStatement">sql</param>
        /// <param name="comm">command</param>
        /// <returns>SqlDataReader</returns>
        public SqlDataReader GetDataReader(string strSql, SqlCommand comm)
        {
            this.OpenConnection();
            SqlDataReader objDataReader = null;
            try
            {
                comm.CommandText = strSql;
                comm.Connection = objConnection;
                objDataReader = comm.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
                this.CloseConnection();
            }
            return objDataReader;
        }
        /// <summary>
        /// exec sql
        /// </summary>
        /// <param name="strSql">sql</param>
        public void ExecuteNonQuery(string strSql, SqlCommand comm)
        {
            this.OpenConnection();
            try
            {
                comm.CommandText = strSql;
                comm.Connection = objConnection;
                comm.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
                throw new Exception(strErr);
            }
            finally
            {
                this.CloseConnection();
            }
        }
        /// <summary>
        /// exec sal
        /// </summary>
        /// <param name="strSql">the sql</param>
        public string ExecuteScalar(string strSql, SqlCommand comm)
        {
            this.OpenConnection();
            try
            {
                comm.CommandText = strSql;
                comm.Connection = objConnection;
                object o = comm.ExecuteScalar();
                if (o == null)
                    return "";
                else
                    return o.ToString();
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
            return "";
        }
        /// <summary>
        ///	get a DataTable by sql
        /// </summary>
        /// <param name="strSql">the sql</param>
        /// <returns>DataSet</returns>
        public DataTable GetDataTable(string strSql, SqlCommand comm)
        {
            this.OpenConnection();
            DataTable objDt = new DataTable();
            try
            {
                comm.CommandText = strSql;
                comm.Connection = objConnection;
                SqlDataAdapter objAdapter = new SqlDataAdapter(comm);
                objAdapter.Fill(objDt);
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
            return objDt;
        }

        //public string GetCountResult(string strSql, Parameters parameter)
        //{
        //    this.OpenConnection();
        //    string result = "0";
        //    try
        //    {
        //        SqlCommand comm = new SqlCommand(strSql, objConnection);
        //        foreach (DictionaryEntry obj in parameter.List)
        //        {
        //            ParameterString pstring = (ParameterString)obj.Value;
        //            comm.Parameters.AddWithValue(pstring.Parameter, pstring.Value);
        //        }
        //        SqlDataReader sdr = comm.ExecuteReader(CommandBehavior.SingleResult);

        //        if (sdr.Read())
        //        {
        //            result = sdr["result"].ToString();
        //        }
        //        sdr.Close();
        //    }
        //    catch (System.Data.SqlClient.SqlException objErr)
        //    {
        //        strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
        //    }
        //    finally
        //    {
        //        this.CloseConnection();
        //    }
        //    return result;
        //}

        //public DataSet GetDataSet(string strSql, Parameters parameter)
        //{
        //    this.OpenConnection();
        //    DataSet objDataSet = new DataSet();
        //    try
        //    {
        //        SqlCommand comm = new SqlCommand(strSql, objConnection);
        //        foreach (DictionaryEntry obj in parameter.List)
        //        {
        //            ParameterString pstring = (ParameterString)obj.Value;
        //            comm.Parameters.AddWithValue(pstring.Parameter, pstring.Value);
        //        }
        //        SqlDataAdapter objAdapter = new SqlDataAdapter(comm);
        //        objAdapter.Fill(objDataSet);
        //    }
        //    catch (System.Data.SqlClient.SqlException objErr)
        //    {
        //        strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
        //    }
        //    finally
        //    {
        //        this.CloseConnection();
        //    }
        //    return objDataSet;
        //}


        /// <summary>
        /// exec sp
        /// </summary>
        /// <param name="strSql">sql</param>
        public void ExecuteProcedure(string strSql, SqlCommand comm)
        {
            this.OpenConnection();
            try
            {
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = strSql;
                comm.Connection = objConnection;
                comm.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }
        }
        /// <summary>
        /// exec sp
        /// </summary>
        /// <param name="strSql">sql</param>
        public DataSet GetDataByProcedure(string strSql, SqlCommand comm)
        {
            DataSet objDataSet = new DataSet();

            this.OpenConnection();
            try
            {
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = strSql;
                comm.Connection = objConnection;

                SqlDataAdapter objAdapter = new SqlDataAdapter(comm);

                objAdapter.Fill(objDataSet);
            }
            catch (System.Data.SqlClient.SqlException objErr)
            {
                strErr = "[Err Source:]" + objErr.Source + "[Err Description:]" + objErr.Message.Replace("\n", "").Replace("\r", "");
            }
            finally
            {
                this.CloseConnection();
            }

            return objDataSet;
        }
        #endregion
    }
}
