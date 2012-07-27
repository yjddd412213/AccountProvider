using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inywhere.Catalog;
using CommonLib;
using CatalogBase;
using System.Data;
using System.Data.SqlClient;

namespace CatalogCommon
{
    public partial class AccountInfoProvider
    {
        DataAccess da = new DataAccess();

        private static AccountInfoProvider m_Instance;

        public static AccountInfoProvider Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new AccountInfoProvider();
                }
                return m_Instance;
            }
        }


        #region User Account related.

        public bool IsUserExist(string userID)
        {
            bool exist = false;

            if (!string.IsNullOrEmpty(userID))
            {
                try
                {
                    string sql = "select * from tb_UserAccountInfo where UserID=@userid";
                    SqlCommand sc = new SqlCommand(sql);
                    sc.Parameters.AddWithValue("@userid", userID);
                    DataTable dtuser = da.GetDataTable(sql, sc);
                    if (dtuser != null && dtuser.Rows != null && dtuser.Rows.Count > 0)
                    {
                        exist = true;
                    }
                }
                catch (Exception e)
                {
                    // ignore.
                }
            }

            return exist;
        }

        public UserAccountInfo GetUserAccountInfo(string userID)        //tested
        {
            UserAccountInfo foundAccountInfo = null;

            if (!string.IsNullOrEmpty(userID))
            {
                DataTable dtuser = GetUserAccountInfoDT(userID, null, null, false);

                if (dtuser != null && dtuser.Rows != null && dtuser.Rows.Count > 0)
                {
                    foundAccountInfo = new UserAccountInfo();
                    try
                    {
                        foundAccountInfo.UserID = userID;

                        DataRowWrapper rowWrapper = new DataRowWrapper(dtuser.Rows[0]);

                        foundAccountInfo.UserName = rowWrapper.GetColumnValueAsString("UserName");
                        foundAccountInfo.Password = rowWrapper.GetTypedColumnValue("Password", typeof(Password)) as Password;
                        foundAccountInfo.ChangedPassword = rowWrapper.GetTypedColumnValue("ChangedPassword", typeof(Password)) as Password;
                        foundAccountInfo.EmailAddress = rowWrapper.GetColumnValueAsString("EmailAddress");
                        foundAccountInfo.AccountType = (AccountType)rowWrapper.GetEnumColumnValue("AccountType", typeof(AccountType));
                        foundAccountInfo.VipPaymentInfo = rowWrapper.GetTypedColumnValue("VipPaymentInfo", typeof(PaymentInfo)) as PaymentInfo;
                        foundAccountInfo.Activated = rowWrapper.GetColumnValueAsBool("Activated");
                        foundAccountInfo.Suspended = rowWrapper.GetColumnValueAsBool("Suspended");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return foundAccountInfo;
        }

        public void UpdateUserAccountInfo(UserAccountInfo userAccountInfo)      //tested
        {
            try
            {
                StringBuilder sql = new StringBuilder("UPDATE tb_UserAccountInfo SET [UserName] =");
                SqlCommand sc = new SqlCommand();
                if (String.IsNullOrEmpty(userAccountInfo.UserName))
                    sql.Append("null");
                else
                    sql.Append("'" + userAccountInfo.UserName + "'");

                sql.Append(",[Password] =");
                if (userAccountInfo.Password == null)
                    sql.Append("null");
                else
                {
                    //sql.Append( "'" + XMLToString(userAccountInfo.Password) + "'";
                    sql.Append("@pwd");
                    sc.Parameters.AddWithValue("@pwd", XMLToString(userAccountInfo.Password));
                }

                sql.Append(",[ChangedPassword] =");
                if (userAccountInfo.ChangedPassword == null)
                    sql.Append("null");
                else
                {
                    //sql.Append( "'" + XMLToString(userAccountInfo.ChangedPassword) + "'";
                    sql.Append("@changedpwd");
                    sc.Parameters.AddWithValue("@changedpwd", XMLToString(userAccountInfo.ChangedPassword));
                }

                sql.Append(",[EmailAddress] =");
                if (String.IsNullOrEmpty(userAccountInfo.EmailAddress))
                    sql.Append("null");
                else
                    sql.Append("'" + userAccountInfo.EmailAddress + "'");

                //accounttype will not be null
                sql.Append(",[AccountType] =");
                //sql.Append( "'" + XMLToString(userAccountInfo.AccountType) + "'";
                sql.Append("@acctype");
                sc.Parameters.AddWithValue("@acctype", userAccountInfo.AccountType.ToString());

                //sql.Append( ",[VipPaymentInfo] =");
                //if (userAccountInfo.VipPaymentInfo == null)
                //    sql.Append( "null");
                //else
                //{
                //    //sql.Append( "'" + XMLToString(userAccountInfo.VipPaymentInfo) + "'";
                //    sql.Append( "@vippayment");
                //    sc.Parameters.AddWithValue("@vippayment", XMLToString(userAccountInfo.VipPaymentInfo));
                //}

                //Activated will not be null
                sql.Append(",[Activated] =");
                sql.Append("'" + userAccountInfo.Activated + "'");

                sql.Append(",[Suspended] =");
                sql.Append("'" + userAccountInfo.Suspended + "'");

                sql.Append(" WHERE [UserId] =@userid");
                sc.Parameters.AddWithValue("@userid", userAccountInfo.UserID);
                da.ExecuteNonQuery(sql.ToString(), sc);
            }
            catch (Exception ex)
            {

            }
        }

        public StatusCode Activate(string userID)       //tested
        {
            string sql = "select * from tb_UserAccountInfo where UserId=@userid";
            SqlCommand sc = new SqlCommand(sql);
            sc.Parameters.AddWithValue("@userid", userID == null ? "" : userID);
            DataTable dtuser = da.GetDataTable(sql, sc);
            StatusCode retStatus = StatusCode.Success;
            if (dtuser != null && dtuser.Rows != null && dtuser.Rows.Count > 0)
            {
                try
                {
                    if (dtuser.Rows[0]["Activated"].ToString() != "True")
                    {
                        sql = "update tb_UserAccountInfo set [Activated]='True' where UserId=@userid";
                        sc = new SqlCommand(sql);
                        sc.Parameters.AddWithValue("@userid", userID);
                        da.ExecuteNonQuery(sql, sc);
                    }
                    else
                        retStatus = StatusCode.AlreadyActivated;
                }
                catch
                {
                    retStatus = StatusCode.Fail;
                }
            }
            else
            {
                retStatus = StatusCode.UserNotFound;
            }
            return retStatus;
        }


        #endregion

        #region Product account related.

        public AccountInfo GetAccountInfo(string userID, string applicationID)      //tested
        {
            AccountInfo foundAccountInfo = null;

            if (userID != null && applicationID != null)
            {
                string sql = "select * from [tb_AccountInfo] where [UserID]=@userid and [ApplicationID]=@appid";
                SqlCommand sc = new SqlCommand(sql);
                sc.Parameters.AddWithValue("@userid", userID);
                sc.Parameters.AddWithValue("@appid", applicationID);
                DataTable dtinfo = da.GetDataTable(sql, sc);
                if (dtinfo != null && dtinfo.Rows != null && dtinfo.Rows.Count > 0)
                {
                    DataRowWrapper rowWrapper = new DataRowWrapper(dtinfo.Rows[0]);

                    foundAccountInfo = new AccountInfo();
                    foundAccountInfo.ApplicationID = applicationID;
                    foundAccountInfo.UserID = userID;

                    foundAccountInfo.UserName = rowWrapper.GetColumnValueAsString("UserName");
                    foundAccountInfo.AccountType = (AccountType)rowWrapper.GetEnumColumnValue("AccountType", typeof(AccountType));
                    foundAccountInfo.Locked = rowWrapper.GetColumnValueAsBool("Locked");
                    foundAccountInfo.LockedDate = dtinfo.Rows[0]["LockedDate"].ToString() != "" ? Convert.ToDateTime(dtinfo.Rows[0]["LockedDate"].ToString()) : Convert.ToDateTime("1900-01-01");
                    foundAccountInfo.LockedDateSpecified = (foundAccountInfo.LockedDate.ToString("yyyy-MM-dd") == "1900-01-01") ? false : true;
                    foundAccountInfo.LockCode = rowWrapper.GetColumnValueAsString("LockCode");
                    foundAccountInfo.ExpiryTime = dtinfo.Rows[0]["ExpiryTime"].ToString() != "" ? Convert.ToDateTime(dtinfo.Rows[0]["ExpiryTime"].ToString()) : Convert.ToDateTime("1900-01-01");
                    foundAccountInfo.ExpiryTimeSpecified = (foundAccountInfo.ExpiryTime.ToString("yyyy-MM-dd") == "1900-01-01") ? false : true;
                    foundAccountInfo.PaymentInfo = rowWrapper.GetTypedColumnValue("PaymentInfo", typeof(PaymentInfo)) as PaymentInfo;

                    foundAccountInfo.DeviceInfoList = rowWrapper.GetTypedColumnValue("DeviceInfos", typeof(DeviceInfoList)) as DeviceInfoList;
                }
            }

            return foundAccountInfo;
        }

        public StatusCode RemoveAccountInfo(string userID, string applicationID)        //tested
        {
            StatusCode retStatus = StatusCode.Success;
            try
            {
                if (!String.IsNullOrEmpty(userID) && !String.IsNullOrEmpty(applicationID))
                {
                    string sql = "delete from [tb_AccountInfo] where [UserID]=@userid and [ApplicationID]=@appid ";
                    SqlCommand sc = new SqlCommand(sql);
                    sc.Parameters.AddWithValue("@userid", userID);
                    sc.Parameters.AddWithValue("@appid", applicationID);
                    da.ExecuteNonQuery(sql, sc);
                }
                else
                {
                    retStatus = StatusCode.Fail;
                }
            }
            catch (Exception ex)
            {

                retStatus = StatusCode.Fail;
            }
            return retStatus;
        }

        public bool Lock(string userID, string applicationID, string lockCode)      //tested
        {
            bool locked = false;
            try
            {
                string sql = "UPDATE [tb_AccountInfo] SET [LockCode] =@lockcode,[Locked]='True'"
                   + " WHERE [ApplicationID] =@appid and [UserID] =@userid";
                SqlCommand sc = new SqlCommand(sql);
                sc.Parameters.AddWithValue("@userid", userID);
                sc.Parameters.AddWithValue("@appid", applicationID);
                sc.Parameters.AddWithValue("@lockcode", lockCode);
                da.ExecuteNonQuery(sql, sc);
                locked = true;
            }
            catch (Exception ex)
            {

                locked = false;
            }

            return locked;
        }

        public bool Unlock(string userID, string applicationID, string lockCode, DeviceInfo devInfo)        //tested
        {
            bool success = false;
            try
            {
                string sql = "UPDATE [tb_AccountInfo] SET [Locked]='False', [LockCode] =null"
                   + " WHERE [ApplicationID] =@appid and [UserID] =@userid and [LockCode] =@lockcode";
                SqlCommand sc = new SqlCommand(sql);
                sc.Parameters.AddWithValue("@userid", userID == null ? "" : userID);
                sc.Parameters.AddWithValue("@appid", applicationID == null ? "" : applicationID);
                sc.Parameters.AddWithValue("@lockcode", lockCode == null ? "" : lockCode);
                da.ExecuteNonQuery(sql, sc);
                success = true;
            }
            catch (Exception ex)
            {

                success = false;
            }
            return success;
        }

        #endregion

        protected string XMLToString(object o)
        {
            if (o != null)
                return CatalogSerializationHelper.Instance.SerializeToString(o);
            else
                return null;
        }
    }
}