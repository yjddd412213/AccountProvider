using System;
using System.Collections.Generic;
using Inywhere.PaymentGateway.DataContract;
using System.Data.SqlClient;
using System.Data;
using CatalogBase;
using Inywhere.Catalog;

namespace CatalogCommon
{
    public partial class AccountInfoProvider
    {
        private const string m_SavePaymentTransactionData = "SavePaymentTransactionData";
        private const string m_GetPaymentTransactionData = "GetPaymentTransactionData";
        private const string m_GetProducts = "GetProducts";
        private const string m_GetTermsByProduct = "GetTermsByProduct";

        public PaymentTransactionData GetPaymentTransactionData(string TransactionId)
        {
            PaymentTransactionData result = null;
            //string sql = "SELECT * FROM PaymentTransactionInfo where IsPay='True' and TransactionId='" + TransactionId + "'";
            string sql = "SELECT Pd.TransactionId, Pd.InywhereId, Pd.ProductTermId, Pd.AccountType, Pd.ChargeDate, CAST(Pd.CustomerInfo AS VARCHAR(MAX)) AS CustomerInfo, CAST(Pd.ChargeAccount AS VARCHAR(MAX)) AS ChargeAccount, CAST(Pd.TransactionResult AS VARCHAR(MAX)) AS TransactionResult, CAST(Pd.TransactionSettings AS VARCHAR(MAX)) AS TransactionSettings, CAST(Pd.ProductInfo AS VARCHAR(MAX)) AS ProductInfo" +
                " ,Pd.InAppPurchase FROM	[dbo].[PaymentTransactionInfo] AS Pd " +
                " LEFT JOIN [dbo].[ProductsTerm] AS Pt ON Pd.ProductTermId = Pt.Id" +
                " LEFT JOIN [dbo].[Products] AS P ON Pt.ProductId = P.ProductId " +
                " LEFT JOIN [dbo].[Terms] AS T ON Pt.TermId = T.TermId" +
                " WHERE Pd.TransactionId ='" + TransactionId + "'";
            var reader = da.GetDataReader(sql);
            if (reader.Read())
            {
                if (reader.FieldCount > 0)
                {
                    result = new PaymentTransactionData();
                    result.TransactionId = reader["TransactionId"].ToString();
                    result.InywhereId = reader["InywhereId"].ToString();
                    result.ChargeDate = DateTime.Parse(reader["ChargeDate"].ToString());
                    if (reader["ChargeAccount"] != null && reader["ChargeAccount"].ToString() != "")
                        result.AccountData = SerializeHelper<BaseChargeAccountData>.Deserialize(reader["ChargeAccount"].ToString());
                    if (reader["CustomerInfo"] != null && reader["CustomerInfo"].ToString() != "")
                        result.CustomerData = SerializeHelper<CustomerData>.Deserialize(reader["CustomerInfo"].ToString());
                    if (reader["ProductInfo"] != null && reader["ProductInfo"].ToString() != "")
                        result.ProductData = SerializeHelper<ProductData>.Deserialize(reader["ProductInfo"].ToString());
                    if (reader["TransactionResult"] != null && reader["TransactionResult"].ToString() != "")
                        result.TransactionResult = SerializeHelper<TransactionResult>.Deserialize(reader["TransactionResult"].ToString());
                    if (reader["TransactionSettings"] != null && reader["TransactionSettings"].ToString() != "")
                        result.TransactionSettings = SerializeHelper<TransactionSettings>.Deserialize(reader["TransactionSettings"].ToString());
                }
            }
            reader.Close();
            return result;
        }

        public bool SavePaymentTransactionData(PaymentTransactionData data)
        {
            using (SqlConnection conn = new SqlConnection(da.dbConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(m_SavePaymentTransactionData, conn))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Unable to connect to InywherePaymentInfo DB using connect string: {0}", conn.ConnectionString));
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        return false;
                    }

                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter[] callParameters = new SqlParameter[12];
                    callParameters[0] = new SqlParameter();
                    callParameters[0].ParameterName = "@TransactionId";
                    callParameters[0].IsNullable = false;
                    callParameters[0].SqlDbType = SqlDbType.VarChar;
                    callParameters[0].Direction = ParameterDirection.Input;
                    callParameters[0].Value = data.TransactionId;

                    callParameters[1] = new SqlParameter();
                    callParameters[1].ParameterName = "@InywhereId";
                    callParameters[1].IsNullable = false;
                    callParameters[1].SqlDbType = SqlDbType.VarChar;
                    callParameters[1].Direction = ParameterDirection.Input;
                    callParameters[1].Value = data.InywhereId;

                    callParameters[2] = new SqlParameter();
                    callParameters[2].ParameterName = "@ProductTermId";
                    callParameters[2].IsNullable = false;
                    callParameters[2].SqlDbType = SqlDbType.VarChar;
                    callParameters[2].Direction = ParameterDirection.Input;
                    callParameters[2].Value = data.ProductData.ProductTermId;

                    callParameters[3] = new SqlParameter();
                    callParameters[3].ParameterName = "@ChargeDate";
                    callParameters[3].IsNullable = false;
                    callParameters[3].SqlDbType = SqlDbType.DateTime;
                    callParameters[3].Direction = ParameterDirection.Input;
                    callParameters[3].Value = data.ChargeDate;

                    callParameters[4] = new SqlParameter();
                    callParameters[4].ParameterName = "@CustomerInfo";
                    callParameters[4].IsNullable = false;
                    callParameters[4].SqlDbType = SqlDbType.VarChar;
                    callParameters[4].Direction = ParameterDirection.Input;
                    callParameters[4].Value = SerializeHelper<CustomerData>.Serialize(data.CustomerData);

                    callParameters[5] = new SqlParameter();
                    callParameters[5].ParameterName = "@ChargeAccount";
                    callParameters[5].IsNullable = false;
                    callParameters[5].SqlDbType = SqlDbType.VarChar;
                    callParameters[5].Direction = ParameterDirection.Input;
                    callParameters[5].Value = SerializeHelper<BaseChargeAccountData>.Serialize(data.AccountData);

                    callParameters[6] = new SqlParameter();
                    callParameters[6].ParameterName = "@TransactionResult";
                    callParameters[6].IsNullable = false;
                    callParameters[6].SqlDbType = SqlDbType.VarChar;
                    callParameters[6].Direction = ParameterDirection.Input;
                    callParameters[6].Value = SerializeHelper<TransactionResult>.Serialize(data.TransactionResult);

                    callParameters[7] = new SqlParameter();
                    callParameters[7].ParameterName = "@TransactionSettings";
                    callParameters[7].IsNullable = false;
                    callParameters[7].SqlDbType = SqlDbType.VarChar;
                    callParameters[7].Direction = ParameterDirection.Input;
                    callParameters[7].Value = SerializeHelper<TransactionSettings>.Serialize(data.TransactionSettings);

                    callParameters[8] = new SqlParameter();
                    callParameters[8].ParameterName = "@AccountType";
                    callParameters[8].IsNullable = false;
                    callParameters[8].SqlDbType = SqlDbType.VarChar;
                    callParameters[8].Direction = ParameterDirection.Input;
                    callParameters[8].Value = data.AccountData.GetType().ToString();

                    callParameters[9] = new SqlParameter();
                    callParameters[9].ParameterName = "@ProductInfo";
                    callParameters[9].IsNullable = false;
                    callParameters[9].SqlDbType = SqlDbType.VarChar;
                    callParameters[9].Direction = ParameterDirection.Input;
                    callParameters[9].Value = SerializeHelper<ProductData>.Serialize(data.ProductData);

                    callParameters[10] = new SqlParameter();
                    callParameters[10].ParameterName = "@IsPay";
                    callParameters[10].IsNullable = true;
                    callParameters[10].SqlDbType = SqlDbType.Bit;
                    callParameters[10].Direction = ParameterDirection.Input;
                    callParameters[10].Value = data.TransactionResult.Suceeded;

                    callParameters[11] = new SqlParameter();
                    callParameters[11].ParameterName = "@InAppPurchase";
                    callParameters[11].IsNullable = true;
                    callParameters[11].SqlDbType = SqlDbType.Bit;
                    callParameters[11].Direction = ParameterDirection.Input;
                    callParameters[11].Value = false;
                    cmd.Parameters.AddRange(callParameters);

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int returnCode = int.Parse(reader[0].ToString());
                        if (returnCode == 0)
                        {
                            return true;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("Not able to insert transaction data, error code {0}", returnCode));
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool SavePaymentTransactionData(PaymentInfo paymentinfo)
        {
            PaymentTransactionData data = AccountInfoProvider.PaymentInfoToPaymentTransactionData(paymentinfo);
            return SavePaymentTransactionData(data);
        }

        public List<Product> GetProducts()
        {
            List<Product> result = new List<Product>();
            using (SqlConnection conn = new SqlConnection(da.dbConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(m_GetProducts, conn))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Unable to connect to InywherePaymentInfo DB using connect string: {0}", conn.ConnectionString));
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        return null;
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.FieldCount > 1)
                        {
                            result.Add(
                                new Product()
                                {
                                    ProductId = reader[0].ToString(),
                                    ProductName = reader[1].ToString(),
                                    IsVip = bool.Parse(reader[2].ToString())
                                }
                                );
                        }
                    }
                }
            }
            return result;
        }

        public List<ProductData> GetTermsByProduct(string productName)
        {
            List<ProductData> result = new List<ProductData>();
            using (SqlConnection conn = new SqlConnection(da.dbConnection.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(m_GetTermsByProduct, conn))
                {
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Unable to connect to InywherePaymentInfo DB using connect string: {0}", conn.ConnectionString));
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                        return null;
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter callParameter = new SqlParameter();
                    callParameter.ParameterName = "@Productname";
                    callParameter.IsNullable = false;
                    callParameter.SqlDbType = SqlDbType.VarChar;
                    callParameter.Direction = ParameterDirection.Input;
                    callParameter.Value = productName == null ? "" : productName;
                    cmd.Parameters.Add(callParameter);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.FieldCount > 1)
                        {
                            result.Add(
                                new ProductData()
                                {
                                    ProductTermId = reader["ProductTermId"].ToString(),
                                    Amount = decimal.Parse(reader["Amount"].ToString()),
                                    Product = new Product() { ProductId = reader["productId"].ToString(), ProductName = reader["ProductName"].ToString(), IsVip = bool.Parse(reader["IsVip"].ToString()) },
                                    Term = new Term() { TermId = int.Parse(reader["TermId"].ToString()), TermType = reader["Type"].ToString(), Description = reader["Description"].ToString() }
                                }
                                );
                        }
                    }
                }
            }
            return result;
        }
    }
}
