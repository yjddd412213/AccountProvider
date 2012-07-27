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
        public StatusCode Register(RegistrationInfo registrationInfo)
        {
            StatusCode statusCode = StatusCode.Fail;

            if (registrationInfo != null && registrationInfo.UserID != null && registrationInfo.Password != null)
            {
                if (!AccountInfoProvider.Instance.IsUserExist(registrationInfo.UserID))
                {
                    UserAccountInfo userAccountInfo = new UserAccountInfo();
                    userAccountInfo.UserID = registrationInfo.UserID;
                    userAccountInfo.Password = registrationInfo.Password;
                    userAccountInfo.EmailAddress = registrationInfo.EmailAddress;

                    if (userAccountInfo != null)
                    {
                        statusCode = AddUserAccountInfo(userAccountInfo);
                    }
                    else
                    {
                        statusCode = StatusCode.Invalid;
                    }
                }
                else
                {
                    statusCode = StatusCode.AlreadyExist;
                }
            }
            else
            {
                statusCode = StatusCode.InvalidData;
            }

            return statusCode;
        }

        public bool Authenticate(string userID, Password password)
        {
            bool authenticated = false;

            if (userID != null && password != null)
            {
                string sql = "select * from tb_UserAccountInfo where UserID=@userid";
                SqlCommand sc = new SqlCommand(sql);
                sc.Parameters.AddWithValue("@userid", userID);
                DataTable dtuser = da.GetDataTable(sql, sc);
                if (dtuser != null && dtuser.Rows != null && dtuser.Rows.Count > 0)
                {
                    try
                    {
                        DataRowWrapper rowWrapper = new DataRowWrapper(dtuser.Rows[0]);
                        Password pwd = rowWrapper.GetTypedColumnValue("Password", typeof(Password)) as Password;
                        if (pwd != null)
                        {
                            if (password.Content == pwd.Content)
                            {
                                authenticated = true;
                            }
                            else
                            {
                                authenticated = false;
                            }
                            // authenticated = password.Equals(pwd);
                        }

                        if (!authenticated)
                        {
                            Password changedPassword = rowWrapper.GetTypedColumnValue("ChangedPassword", typeof(Password)) as Password;

                            if (changedPassword != null)
                            {
                                authenticated = password.Equals(changedPassword);

                                if (authenticated)
                                {
                                    AccountInfoProvider.Instance.ChangePassword(userID, changedPassword);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }

            return authenticated;
        }

        public UserAccountSummary GetUserAccountSummary(string userID)
        {
            UserAccountSummary summary = null;
            if (userID != null)
            {
                string sql = "select * from [tb_AccountInfo] where [UserID]=@userid";
                SqlCommand sc = new SqlCommand(sql);
                sc.Parameters.AddWithValue("@userid", userID);
                DataTable dtinfo = da.GetDataTable(sql, sc);
                if (dtinfo != null && dtinfo.Rows != null)
                {
                    summary = new UserAccountSummary();
                    summary.UserID = userID;

                    foreach (DataRow row in dtinfo.Rows)
                    {
                        DataRowWrapper rowWrapper = new DataRowWrapper(row);

                        AppAccountInfo appAccountInfo = new AppAccountInfo();
                        appAccountInfo.ApplicationID = rowWrapper.GetColumnValueAsString("ApplicationID");

                        appAccountInfo.Locked = rowWrapper.GetColumnValueAsBool("Locked");
                        appAccountInfo.ExpiryTime = dtinfo.Rows[0]["ExpiryTime"].ToString() != "" ? Convert.ToDateTime(dtinfo.Rows[0]["ExpiryTime"].ToString()) : Convert.ToDateTime("1900-01-01");
                        appAccountInfo.ExpiryTimeSpecified = (appAccountInfo.ExpiryTime.ToString("yyyy-MM-dd") == "1900-01-01") ? false : true;

                        summary.AddAppAccountInfo(appAccountInfo);
                    }
                }
            }
            return summary;
        }

        public StatusCode ChangePassword(string userID, Password newPassword)
        {
            StatusCode statusCode = StatusCode.Success;
            try
            {
                string sql = "update tb_UserAccountInfo set [ChangedPassword] = [Password], [Password] = @pwd"
                    + " where  [UserId] =@userid";
                SqlCommand sc = new SqlCommand(sql);
                sc.Parameters.AddWithValue("@pwd", XMLToString(newPassword));
                sc.Parameters.AddWithValue("@userid", userID);
                da.ExecuteNonQuery(sql, sc);
            }
            catch (Exception ex)
            {

                statusCode = StatusCode.Fail;
            }
            return statusCode;
        }

        protected StatusCode AddUserAccountInfo(UserAccountInfo userAccountInfo)
        {
            StatusCode status = StatusCode.Success;

            try
            {
                StringBuilder sql = new StringBuilder("INSERT INTO [tb_UserAccountInfo] ([UserId],[UserName],[Password],[ChangedPassword]"
                        + ",[EmailAddress],[AccountType],[VipPaymentInfo],[Activated],[CreationTime]) VALUES"
                        + "('" + userAccountInfo.UserID + "'");

                SqlCommand sc = new SqlCommand();
                if (String.IsNullOrEmpty(userAccountInfo.UserName))
                    sql.Append(",null");
                else
                    sql.Append(",'" + userAccountInfo.UserName + "'");

                if (userAccountInfo.Password == null)
                    sql.Append(",null");
                else
                {
                    sql.Append(",@pwd");
                    sc.Parameters.AddWithValue("@pwd", XMLToString(userAccountInfo.Password));
                }

                if (userAccountInfo.ChangedPassword == null)
                    sql.Append(",null");
                else
                {
                    //sql.Append( ",'" + XMLToString(userAccountInfo.ChangedPassword) + "'";
                    sql.Append(",@changedpwd");
                    sc.Parameters.AddWithValue("@changedpwd", XMLToString(userAccountInfo.ChangedPassword));
                }

                if (String.IsNullOrEmpty(userAccountInfo.EmailAddress))
                    sql.Append(",null");
                else
                    sql.Append(",'" + userAccountInfo.EmailAddress + "'");

                //accounttype will not be null
                sql.Append(",@acctype");
                sc.Parameters.AddWithValue("@acctype", userAccountInfo.AccountType.ToString());

                if (userAccountInfo.VipPaymentInfo == null)
                    sql.Append(",null");
                else
                {
                    //sql.Append( ",'" + XMLToString(userAccountInfo.VipPaymentInfo) + "'";
                    sql.Append(",@vippayment");
                    sc.Parameters.AddWithValue("@vippayment", XMLToString(userAccountInfo.VipPaymentInfo));
                }

                //Activated will not be null
                sql.Append(",'" + userAccountInfo.Activated + "','" + da.ExecuteScalar("select getdate()") + "'");
                sql.Append(")");

                da.ExecuteNonQuery(sql.ToString(), sc);
            }
            catch (Exception e)
            {
                status = StatusCode.Fail;
            }

            return status;
        }
    }
}
