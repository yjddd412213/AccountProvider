using System;
using Inywhere.Catalog;
using System.Data;
using CatalogBase;
using System.Collections.Generic;

namespace CatalogCommon
{
    public partial class AccountInfoProvider
    {
        private List<String> terms = new List<string> { "Monthly", "Quarterly", "HalfYear", "Yearly", "VIP" };

        public DataTable getTotalAccount()
        {
            string sql = "select * from tb_UserAccountInfo";
            return da.GetDataTable(sql);
        }

        public DataTable getPaidAccount(string startdate, string enddate)
        {
            string sql = "select count(inywhereid) paid, count(distinct(inywhereid)) num from PaymentTransactionInfo where "
                + " IsPay='True' ";
            if (!String.IsNullOrEmpty(startdate))
                sql += " and ChargeDate >='" + startdate + "'";
            if (!String.IsNullOrEmpty(enddate))
                sql += " and ChargeDate <='" + Convert.ToDateTime(enddate).AddDays(1) + "'";
            return da.GetDataTable(sql);
        }

        public DataTable getTransactionInfo(string id, string userid, string startdate, string enddate)
        {
            string sql = "SELECT Pd.TransactionId, Pd.InywhereId, Pd.ProductTermId, Pt.ProductId, Pd.IsPay, Pd.ChargeDate" +
                ",  Amount,Pd.InAppPurchase FROM	[dbo].[PaymentTransactionInfo] AS Pd " +
                " LEFT JOIN [dbo].[ProductsTerm] AS Pt ON Pd.ProductTermId = Pt.Id" +
                " LEFT JOIN [dbo].[Products] AS P ON Pt.ProductId = P.ProductId " +
                " LEFT JOIN [dbo].[Terms] AS T ON Pt.TermId = T.TermId" +
                " where IsPay='True' ";
            if (!String.IsNullOrEmpty(id))
                sql += " and Pd.TransactionId like '%" + id + "%'";
            if (!String.IsNullOrEmpty(userid))
                sql += " and Pd.InywhereId like '%" + userid + "%'";
            if (!String.IsNullOrEmpty(startdate))
                sql += " and ChargeDate >='" + startdate + "'";
            if (!String.IsNullOrEmpty(enddate))
                sql += " and ChargeDate <='" + Convert.ToDateTime(enddate).AddDays(1) + "'";
            sql += " order by ChargeDate desc";
            return da.GetDataTable(sql);
        }

        public DataTable GetUserAccountInfoDT(string userID, string startdate, string enddate, bool ispaid)
        {
            string sql = "select * from tb_UserAccountInfo where 1=1 ";
            if (!String.IsNullOrEmpty(userID))
                sql += " and UserID like '%" + userID + "%'";
            if (!String.IsNullOrEmpty(startdate))
                sql += " and CreationTime>='" + startdate + "'";
            if (!String.IsNullOrEmpty(enddate))
                sql += " and CreationTime <='" + Convert.ToDateTime(enddate).AddDays(1) + "'";
            if (ispaid)
                sql += " and UserID in (select distinct(InywhereId) from PaymentTransactionInfo)";
            return da.GetDataTable(sql);
        }

        public DataTable GetAccountInfoDT(string userID)
        {
            string sql = "select * from tb_AccountInfo";
            if (!String.IsNullOrEmpty(userID))
                sql += " where UserID like '%" + userID + "%'";
            DataTable dt = da.GetDataTable(sql);
            dt.Columns.Add("PaymentType");
            dt.Columns.Add("PaymentTime");

            PaymentInfo payment = new PaymentInfo();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                try
                {
                    payment = SerializeHelper<PaymentInfo>.Deserialize(dt.Rows[i]["PaymentInfo"].ToString());
                    dt.Rows[i]["PaymentType"] = payment.PaymentType;
                    dt.Rows[i]["PaymentTime"] = payment.PaymentTime;
                }
                catch
                {
                    dt.Rows[i]["PaymentType"] = "";
                    dt.Rows[i]["PaymentTime"] = "";
                }
            }
            return dt;
        }

        public void UpdateUserAccountInfo(string userID, string activated, string suspended)
        {
            string sql = "update tb_UserAccountInfo set Activated='" + activated
                + "', Suspended='" + suspended + "' where UserId='" + userID + "'";
            da.ExecuteNonQuery(sql);
        }

        public void UpdateAccountInfo(string userID, string appID, string Locked)
        {
            string sql = "update tb_AccountInfo set Locked='" + Locked
                           + "' where UserId='" + userID + "' and ApplicationID='" + appID + "'";
            da.ExecuteNonQuery(sql);
        }

        public DataTable GetStatisticsInfo(string startdate, string enddate)
        {
            DataTable dt = new DataTable();
            string sql = "";

            sql = "select ApplicationID Product,(select count(ApplicationID) from tb_accountinfo c where c.ApplicationID=a.ApplicationID) Total," +
                    "(select count(AccountType) from tb_accountinfo b where b.AccountType<>'Free' and b.ApplicationID=a.ApplicationID) Paid " +
                    "from tb_accountinfo a,PaymentTransactionInfo b where UserID=InywhereId ";
            if (!String.IsNullOrEmpty(startdate))
                sql += " and ChargeDate >='" + startdate + "'";
            if (!String.IsNullOrEmpty(enddate))
                sql += " and ChargeDate <='" + Convert.ToDateTime(enddate).AddDays(1) + "'";
            sql += " group by a.ApplicationID";
            dt = da.GetDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt.Columns.Add(PaymentType.Monthly.ToString());
                dt.Columns.Add(PaymentType.Quarterly.ToString());
                dt.Columns.Add(PaymentType.HalfYear.ToString());
                dt.Columns.Add(PaymentType.Yearly.ToString());
                dt.Columns.Add(PaymentType.VIP.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["Monthly"] = getCount(dt.Rows[i]["Product"].ToString(), "Monthly", startdate, enddate);
                    dt.Rows[i]["Quarterly"] = getCount(dt.Rows[i]["Product"].ToString(), "Quarterly", startdate, enddate);
                    dt.Rows[i]["HalfYear"] = getCount(dt.Rows[i]["Product"].ToString(), "HalfYear", startdate, enddate);
                    dt.Rows[i]["Yearly"] = getCount(dt.Rows[i]["Product"].ToString(), "Yearly", startdate, enddate);
                    dt.Rows[i]["VIP"] = getCount(dt.Rows[i]["Product"].ToString(), "VIP", startdate, enddate);
                }
            }
            return dt;
        }

        private string getCount(string product, string term, string startdate, string enddate)
        {
            string sql = "select InywhereId from PaymentTransactionInfo,ProductsTerm pt,Terms t where ProductTermId=Id and pt.termid=t.termid and pt.productid='" + product + "' and t.type='" + term + "'";
            if (!String.IsNullOrEmpty(startdate))
                sql += " and ChargeDate >='" + startdate + "'";
            if (!String.IsNullOrEmpty(enddate))
                sql += " and ChargeDate <='" + Convert.ToDateTime(enddate).AddDays(1) + "'";
            return da.GetDataTable(sql).Rows.Count.ToString();
        }
    }
}
