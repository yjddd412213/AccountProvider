using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlTypes;

namespace CatalogCommon
{
    public class DataRowWrapper
    {
        public DataRow m_Row;

        public DataRowWrapper(DataRow row)
        {
            m_Row = row;
        }

        public object GetColumnValue(string colName)
        {
            object val = null;
            try
            {
                val = m_Row[colName];
            }
            catch (Exception e)
            {
                // ignore.
            }
            return val;
        }

        public string GetColumnValueAsString(string colName)
        {
            string val = null;
            try
            {
                val = m_Row[colName] as string;
            }
            catch (Exception e)
            {
                // ignore.
            }


            return val;
        }

        public decimal? GetColumnValueDecimal(string colName)
        {
            decimal? val = null;
            try
            {
                val = m_Row.Field<decimal?>(colName);
            }
            catch (Exception e)
            {
                // ignore.
            }


            return val;
        }

        public bool GetColumnValueAsBool(string colName)
        {
            bool? val = null;
            try
            {
                val = m_Row.Field<bool?>(colName);
            }
            catch (Exception e)
            {
                // ignore.
            }

            bool retVal = false;

            if (val != null)
            {
                retVal = val.GetValueOrDefault();
            }

            return retVal;
        }

        //public bool GetColumnValueAsBool(string colName)
        //{
        //    string val = null;
        //    try
        //    {
        //        val = m_Row[colName] as string;
        //    }
        //    catch (Exception e)
        //    {
        //        // ignore.
        //    }

        //    bool retBool = false;
        //    if (val != null)
        //    {
        //        try
        //        {
        //            retBool = Convert.ToBoolean(val);
        //        }
        //        catch (Exception e)
        //        {
        //            // ignore.
        //        }
        //    }

        //    return retBool;
        //}

        public object GetTypedColumnValue(string colName, Type type)
        {
            string strVal = null;
            try
            {
                strVal = m_Row[colName] as string;
            }
            catch (Exception e)
            {
                // ignore.
            }

            object retObj = null;

            if (strVal != null)
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    byte[] buffer = UTF8Encoding.UTF8.GetBytes(strVal);
                    MemoryStream stream = new MemoryStream(buffer);
                    retObj = xs.Deserialize(stream);
                }
                catch (Exception e)
                {
                    // ignore
                }
            }

            return retObj;
        }

        public object GetEnumColumnValue(string colName, Type type)
        {
            string strVal = null;
            try
            {
                strVal = m_Row[colName] as string;
            }
            catch (Exception e)
            {
                // ignore.
            }

            object retObj = null;

            if (strVal != null)
            {
                try
                {
                    retObj = Enum.Parse(type, strVal);
                }
                catch (Exception e)
                {
                    // ignore
                }
            }

            return retObj;
        }
    }
}
