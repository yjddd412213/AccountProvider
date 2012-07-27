using System;
using System.Data;

namespace CommonLib
{
    public class Setting
    {
        DataAccess da = new DataAccess();

        private static Setting m_Instance;

        public static Setting Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Setting();
                }
                return m_Instance;
            }
        }

        public DataTable GetSettings()
        {
            return da.GetDataTable("SELECT * FROM [Setting]");
        }

        public void UpdateSetting(string key, string value)
        {
            string sql = "UPDATE [Setting] SET [value] = '" + value + "'"
                    + "WHERE [key] ='" + key + "'";
            da.ExecuteNonQuery(sql);
        }

        public void DeleteSetting(string key)
        {
            DataAccess da = new DataAccess();
            string sql = "DELETE from [Setting] "
                    + "WHERE [key] ='" + key + "'";
            da.ExecuteNonQuery(sql);
        }

        public string GetSetting(string key)
        {
            return da.ExecuteScalar("SELECT  [value] FROM [Setting] where [key]='" + key + "'");
        }

        public bool AddSetting(string key, string value)
        {
            string val = GetSetting(key);
            if (String.IsNullOrEmpty(val))
            {
                string sql = "INSERT INTO [Setting] ([key],[value]) VALUES"
                            + "('" + key + "','" + value + "')";
                da.ExecuteNonQuery(sql);
                return true;
            }
            else
                return false;
        }
    }
}
