using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLib;

namespace CatalogBase
{
    public class CatalogSerializationHelper : SerializationHelper
    {
        private static CatalogSerializationHelper m_Instance;

        public static CatalogSerializationHelper Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new CatalogSerializationHelper();
                }
                return m_Instance;
            }
        }

        protected override string NameSpacePrefix
        {
            get { return "Inywhere.Catalog."; }
        }

        protected override Type GetType(string typeName)
        {
            return Type.GetType(typeName);
        }

    }
}
