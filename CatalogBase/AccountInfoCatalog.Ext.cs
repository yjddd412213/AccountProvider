using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;


namespace Inywhere.Catalog
{
    public partial class UserAccountSummary
    {
        public void AddAppAccountInfo(AppAccountInfo appAccountInfo)
        {
            if (appAccountInfo != null)
            {
                DynamicArray<AppAccountInfo> dynamicArray = new DynamicArray<AppAccountInfo>();
                AppAccountInfos = dynamicArray.AddToArray(AppAccountInfos, appAccountInfo);
            }
        }
    }

    public partial class AppPurchaseInfo
    {
        public PurchaseInfo ToPurchaseInfo()
        {
            PurchaseInfo info = new PurchaseInfo();

            info.PaymentOptions = PaymentOptions;

            return info;
        }

        public void AddPaymentOption(PaymentOption option)
        {
            if (option != null)
            {
                DynamicArray<PaymentOption> dynamicArray = new DynamicArray<PaymentOption>();
                PaymentOptions = dynamicArray.AddToArray(PaymentOptions, option);
            }
        }

        public void AddPaymentOptionRange(ICollection<PaymentOption> options)
        {
            if (options != null)
            {
                DynamicArray<PaymentOption> dynamicArray = new DynamicArray<PaymentOption>();
                PaymentOptions = dynamicArray.AddToArrayRange(PaymentOptions, options);
            }
        }
    }

    public partial class PaymentInfo
    {
        public static TimeSpan TimeSpanForPaymentType(PaymentType payType)
        {
            TimeSpan span = TimeSpan.MinValue;

            switch (payType)
            {
                case PaymentType.Monthly:
                    span = TimeSpan.FromDays(31);
                    break;
                case PaymentType.Quarterly:
                    span = TimeSpan.FromDays(31 * 3);
                    break;
                case PaymentType.HalfYear:
                    span = TimeSpan.FromDays(31 * 6);
                    break;
                case PaymentType.Yearly:
                    span = TimeSpan.FromDays(365);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return span;
        }

        public void SetPrice(string amount)
        {
            if (amount != null)
            {
                Price = new Price();
                Price.Amount = amount;
            }
        }

        public void SetPaymentType(string term)
        {
            if (term != null)
            {
                try
                {
                    PaymentType = (PaymentType)(Enum.Parse(typeof(PaymentType), term));
                }
                catch (Exception e)
                {
                    // ignore.
                }
            }
        }
    }

}
