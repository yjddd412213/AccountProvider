using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Inywhere.Catalog;
using CommonLib;
using CatalogBase;
using System.Data;
using System.Data.SqlClient;
using Inywhere.PaymentGateway.DataContract;

namespace CatalogCommon
{
    public partial class AccountInfoProvider
    {
        const string prefix = "inywhere_";
        public StatusCode AddAccountInfo(AccountInfo accountInfo)       //tested
        {
            StatusCode retStatus = StatusCode.Success;
            try
            {
                StringBuilder sql = new StringBuilder("INSERT INTO [tb_AccountInfo] ([ApplicationID],[UserID],[UserName],[AccountType],[Locked],"
                        + "[LockedDate],[LockCode],[ExpiryTime],[PaymentInfo]) VALUES(");
                SqlCommand sc = new SqlCommand();

                if (String.IsNullOrEmpty(accountInfo.ApplicationID))
                    sql.Append("null,");
                else
                    sql.Append("'" + accountInfo.ApplicationID + "',");

                if (String.IsNullOrEmpty(accountInfo.UserID))
                    sql.Append("null,");
                else
                    sql.Append("'" + accountInfo.UserID + "',");

                if (String.IsNullOrEmpty(accountInfo.UserName))
                    sql.Append("null,");
                else
                    sql.Append("'" + accountInfo.UserName + "',");

                sql.Append("@acctype,");
                sc.Parameters.AddWithValue("@acctype", accountInfo.AccountType.ToString());

                //Locked will not be null
                sql.Append("'" + accountInfo.Locked + "',");

                if (accountInfo.LockedDateSpecified && accountInfo.LockedDate != null)
                    sql.Append("'" + accountInfo.LockedDate + "',");
                else
                    sql.Append("null,");

                if (accountInfo.Locked && !String.IsNullOrEmpty(accountInfo.LockCode))
                    sql.Append("'" + accountInfo.LockCode + "',");
                else
                    sql.Append("null,");

                if (accountInfo.ExpiryTimeSpecified && accountInfo.ExpiryTime != null)
                    sql.Append("'" + accountInfo.ExpiryTime + "',");
                else
                    sql.Append("null,");

                if (accountInfo.PaymentInfo != null)
                {
                    //sql.Append( "'" + XMLToString(accountInfo.PaymentInfo) + "',";
                    sql.Append("@paymentinfo");
                    sc.Parameters.AddWithValue("@paymentinfo", XMLToString(accountInfo.PaymentInfo));
                }
                else
                    sql.Append("null");

                sql.Append(")");

                da.ExecuteNonQuery(sql.ToString(), sc);
            }
            catch (Exception ex)
            {
                retStatus = StatusCode.Fail;
            }
            return retStatus;
        }

        public bool HasApplicationAccountInfo(string userID, string applicationID)
        {
            bool exist = false;

            if (!string.IsNullOrEmpty(userID))
            {
                try
                {
                    string sql = "select * from tb_AccountInfo where UserID=@userid and [ApplicationID]=@appid";
                    SqlCommand sc = new SqlCommand(sql);
                    sc.Parameters.AddWithValue("@userid", userID);
                    sc.Parameters.AddWithValue("@appid", applicationID);
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

        public StatusCode UpdateAccountInfo(AccountInfo accountInfo)        //tested
        {
            StatusCode retStatus = StatusCode.Success;
            try
            {
                StringBuilder sql = new StringBuilder("UPDATE [tb_AccountInfo] SET [UserName] =");
                SqlCommand sc = new SqlCommand();
                if (String.IsNullOrEmpty(accountInfo.UserName))
                    sql.Append("null,");
                else
                    sql.Append("'" + accountInfo.UserName + "',");

                //accounttype will not be null
                sql.Append("[AccountType] =");
                //sql.Append( "'" + XMLToString(accountInfo.AccountType) + "',";
                sql.Append("@acctype,");
                sc.Parameters.AddWithValue("@acctype", accountInfo.AccountType.ToString());

                //Locked will not be null
                sql.Append("[Locked]=");
                sql.Append("'" + accountInfo.Locked + "',");

                sql.Append("[LockedDate] =");
                if (accountInfo.LockedDateSpecified && accountInfo.LockedDate != null)
                    sql.Append("'" + accountInfo.LockedDate + "',");
                else
                    sql.Append("null,");

                sql.Append("[LockCode] =");
                if (accountInfo.Locked && !String.IsNullOrEmpty(accountInfo.LockCode))
                    sql.Append("'" + accountInfo.LockCode + "',");
                else
                    sql.Append("null,");

                sql.Append("[ExpiryTime] =");
                if (accountInfo.ExpiryTimeSpecified && accountInfo.ExpiryTime != null)
                    sql.Append("'" + accountInfo.ExpiryTime + "',");
                else
                    sql.Append("null,");

                sql.Append("[PaymentInfo]=");
                if (accountInfo.PaymentInfo != null)
                {
                    //sql.Append( "'" + XMLToString(accountInfo.PaymentInfo) + "',";
                    sql.Append("@paymentinfo");
                    sc.Parameters.AddWithValue("@paymentinfo", XMLToString(accountInfo.PaymentInfo));
                }
                else
                    sql.Append("null");

                sql.Append(" WHERE [ApplicationID] ='" + accountInfo.ApplicationID
                    + "' and [UserID] ='" + accountInfo.UserID + "'");

                da.ExecuteNonQuery(sql.ToString(), sc);
            }
            catch (Exception ex)
            {

                retStatus = StatusCode.Fail;
            }
            return retStatus;
        }

        public StatusCode UpdateApplicationPayment(string userID, string applicationID, PaymentInfo paymentInfo)
        {
            StatusCode status = StatusCode.Fail;
            if (paymentInfo != null)
            {
                if (paymentInfo.PaymentType == PaymentType.VIP)
                {
                    applicationID = "VIP";
                    if (HasApplicationAccountInfo(userID, applicationID))
                    {
                        if (UpdatePurchaseInfo(userID, applicationID, paymentInfo))
                        {
                            status = StatusCode.Success;
                        }
                    }
                    else
                    {
                        AccountInfo vipAccountInfo = new AccountInfo();
                        vipAccountInfo.ApplicationID = applicationID;
                        vipAccountInfo.UserID = userID;
                        vipAccountInfo.AccountType = AccountType.VIP;
                        vipAccountInfo.PaymentInfo = paymentInfo;

                        status = AddAccountInfo(vipAccountInfo);
                    }

                }
                else
                {
                    if (HasApplicationAccountInfo(userID, applicationID))
                    {
                        if (UpdatePurchaseInfo(userID, applicationID, paymentInfo))
                        {
                            status = StatusCode.Success;
                        }
                    }
                    else
                    {
                        AccountInfo paidAccountInfo = new AccountInfo();
                        paidAccountInfo.ApplicationID = applicationID;
                        paidAccountInfo.UserID = userID;
                        paidAccountInfo.AccountType = AccountType.Paid;
                        paidAccountInfo.PaymentInfo = paymentInfo;

                        status = AddAccountInfo(paidAccountInfo);
                    }
                }
            }
            return status;
        }

        public bool UpdatePurchaseInfo(string userID, string applicationID, PaymentInfo paymentInfo)        //tested
        {
            bool updated = false;
            if (userID != null && applicationID != null && paymentInfo != null)
            {
                try
                {
                    string sql = "UPDATE [tb_AccountInfo] SET [PaymentInfo] =@paymentinfo,[AccountType] =@acctype"
                       + " WHERE [ApplicationID] =@appid and [UserID] =@userid";
                    SqlCommand sc = new SqlCommand(sql);
                    sc.Parameters.AddWithValue("@userid", userID);
                    sc.Parameters.AddWithValue("@appid", applicationID);
                    sc.Parameters.AddWithValue("@acctype", PaymentTypeToAccountType(paymentInfo.PaymentType).ToString());
                    sc.Parameters.AddWithValue("@paymentinfo", XMLToString(paymentInfo));
                    da.ExecuteNonQuery(sql, sc);

                    updated = true;
                }
                catch (Exception ex)
                {
                }
            }

            return updated;
        }

        public static AccountType PaymentTypeToAccountType(PaymentType paymentType)
        {
            AccountType accountType = AccountType.Free;
            switch (paymentType)
            {
                case PaymentType.VIP:
                    accountType = AccountType.VIP;
                    break;
                default:
                    accountType = AccountType.Paid;
                    break;
            }
            return accountType;
        }

        public bool PaymentSummaryNotification(string userID, PaymentInfo paymentInfo)
        {
            bool success = false;
            if (userID != null && paymentInfo != null)
            {

                if (UpdateApplicationPayment(userID, paymentInfo.ProductID, paymentInfo) == StatusCode.Success)
                {
                    success = true;
                }

            }

            return success;
        }

        private static PaymentTransactionData PaymentInfoToPaymentTransactionData(PaymentInfo paymentInfo)
        {
            PaymentTransactionData transData = null;

            if (paymentInfo != null)
            {
                transData = new PaymentTransactionData();

                try
                {
                    ProductData productdata = new ProductData();
                    Product product = new Product();
                    product.ProductId = paymentInfo.ProductID;
                    product.ProductName = paymentInfo.ProductName;
                    product.IsVip = paymentInfo.ProductID.ToLower() == "vip";
                    productdata.Product = product;

                    productdata.Amount = Convert.ToDecimal(paymentInfo.Price.Amount);

                    Term term = new Term();
                    DataAccess da = new DataAccess();
                    DataTable dtterm = da.GetDataTable("select * from Terms where [Type]='" + productdata.Term.TermType + "'");
                    term.TermId = Convert.ToInt32(dtterm.Rows[0]["TermId"].ToString());
                    term.TermType = dtterm.Rows[0]["Type"].ToString();
                    term.Description = dtterm.Rows[0]["Description"].ToString();
                    productdata.Term = term;

                    productdata.ProductTermId = prefix + productdata.Product.ProductId.ToLower() + "_" + productdata.Term.Description.ToLower();
                    transData.TransactionId = paymentInfo.TransactionID;
                    transData.ChargeDate = paymentInfo.PaymentTime;
                    transData.ProductData = productdata;
                }
                catch { }
            }

            return transData;
        }

        public static PaymentInfo PaymentTransactionDataToPaymentInfo(PaymentTransactionData transData)
        {
            PaymentInfo paymentInfo = null;

            if (transData != null && transData.ProductData != null)
            {
                paymentInfo = new PaymentInfo();

                try
                {
                    paymentInfo.ProductID = transData.ProductData.Product.ProductId;
                    paymentInfo.ProductName = transData.ProductData.Product.ProductName;
                    paymentInfo.SetPrice(transData.ProductData.Amount.ToString());
                    paymentInfo.TransactionID = transData.TransactionId;
                    paymentInfo.PaymentTime = transData.ChargeDate;
                    paymentInfo.PaymentTimeSpecified = true;
                    paymentInfo.SetPaymentType(transData.ProductData.Term.TermType);

                }
                catch (Exception e)
                {
                    // ignore.
                }
            }

            return paymentInfo;
        }
    }
}
