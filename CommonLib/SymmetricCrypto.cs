using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace CommonLib
{
    public class SymmetricCryptoHelper
    {
        private static SymmetricCryptoHelper m_Instance;

        TripleDESCryptoServiceProvider m_Provider;

        //private readonly string IV = "SuFjcEmp/TE=";
        private const string IV = "IV value";
        //private readonly string Key = "KIPSToILGp6fl+3gXJvMsN4IajizYBBT";
        private const string Key = "Inywhere01234567";        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoHelper"/> class.
        /// </summary>
        private SymmetricCryptoHelper()
        {
            //IV = ConfigurationManager.AppSettings["IV"];
            //Key = ConfigurationManager.AppSettings["Key"];
        }

        public static SymmetricCryptoHelper Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new SymmetricCryptoHelper();
                }
                return m_Instance;
            }
        }
        /// <summary>
        /// Gets the encrypted value.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <returns></returns>
        public string GetEncryptedValue(string inputValue)
        {
            TripleDESCryptoServiceProvider provider = this.GetCryptoProvider();
            // Create a MemoryStream.
            MemoryStream mStream = new MemoryStream();

            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new CryptoStream(mStream,
                provider.CreateEncryptor(), CryptoStreamMode.Write);

            // Convert the passed string to a byte array.
             byte[] toEncrypt = new UTF8Encoding().GetBytes(inputValue);

            // Write the byte array to the crypto stream and flush it.
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();

            // Get an array of bytes from the 
            // MemoryStream that holds the 
            // encrypted data.
            byte[] ret = mStream.ToArray();

            // Close the streams.
            cStream.Close();
            mStream.Close();

            // Return the encrypted buffer.
            return Convert.ToBase64String(ret);
        }

        /// <summary>
        /// Gets the crypto provider.
        /// </summary>
        /// <returns></returns>
        private TripleDESCryptoServiceProvider GetCryptoProvider()
        {
            if (m_Provider == null)
            {
                m_Provider = new TripleDESCryptoServiceProvider();
                m_Provider.IV = Encoding.UTF8.GetBytes(IV); // Convert.FromBase64String(IV);
                m_Provider.Key = Encoding.UTF8.GetBytes(Key); // Convert.FromBase64String(Key);
            }
            return m_Provider;
        }

        /// <summary>
        /// Gets the decrypted value.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <returns></returns>
        public string GetDecryptedValue(string inputValue)
        {
            string clearValue = null;
            if (inputValue != null)
            {
                try
                {
                    TripleDESCryptoServiceProvider provider = this.GetCryptoProvider();
                    byte[] inputEquivalent = Convert.FromBase64String(inputValue);
                    // Create a new MemoryStream.
                    MemoryStream msDecrypt = new MemoryStream();

                    // Create a CryptoStream using the MemoryStream 
                    // and the passed key and initialization vector (IV).
                    CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                        provider.CreateDecryptor(),
                        CryptoStreamMode.Write);

                    using (csDecrypt)
                    {
                        csDecrypt.Write(inputEquivalent, 0, inputEquivalent.Length);
                        csDecrypt.FlushFinalBlock();
                    }

                    //Convert the buffer into a string and return it.
                    clearValue = new UTF8Encoding().GetString(msDecrypt.ToArray());
                }
                catch (Exception e)
                {

                }
            }

            
            return clearValue;
        }
    }
}
