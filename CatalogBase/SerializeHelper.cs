using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CatalogBase
{
    public static class SerializeHelper<T>
    {
        public static string Serialize(T obj)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            xs.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public static T Deserialize(string data)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(data);
            MemoryStream stream = new MemoryStream(buffer);
            return (T)xs.Deserialize(stream);
        }
    }
}
