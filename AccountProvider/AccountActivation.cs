using System;
using CommonLib;

namespace CatalogCommon
{
    /// <summary>
    /// Summary description for AccountActivation
    /// </summary>
    public class AccountActivation
    {
        public static StatusCode Activate(string secretUserID)
        {
            if (secretUserID != null)
            {
                string clearUserID = SymmetricCryptoHelper.Instance.GetDecryptedValue(secretUserID);

                if (clearUserID != null)
                {
                    return AccountInfoProvider.Instance.Activate(clearUserID);
                }
            }
            return StatusCode.Invalid;
        }
    }
}