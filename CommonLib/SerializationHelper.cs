using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CommonLib
{
    public class ParseResult
    {
        private string m_Name;
        private object m_Obj;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public string TypeShortName
        {
            get
            {
                string shortName = null;
                if (m_Name != null)
                {
                    int pos = m_Name.LastIndexOf('.');
                    if (pos != -1)
                    {
                        shortName = m_Name.Substring(pos + 1);
                    }
                    else
                    {
                        shortName = m_Name;
                    }
                }
                return shortName;
            }
        }

        public object Obj
        {
            get { return m_Obj; }
            set { m_Obj = value; }
        }
    }

    public abstract class SerializationHelper
    {
        protected abstract string NameSpacePrefix
        {
            get;
        }

        public string SerializeToString(object obj, Encoding encode)
        {
            string str = null;
            byte[] bytes = SerializeToBytes(obj);

            if (bytes != null)
            {
                str = encode.GetString(bytes, 0, bytes.Length);
            }

            return str;
        }

        public string SerializeToString(object obj)
        {
            return SerializeToString(obj, Encoding.UTF8);
        }

        public byte[] SerializeToBytes(object obj)
        {
            //byte[] bytes = null;

            //if (obj != null /*&& obj.GetType().IsSerializable*/)
            //{
            //    XmlSerializer serializer = new XmlSerializer(obj.GetType());
            //    MemoryStream stream = new MemoryStream();
            //    serializer.Serialize(stream, obj);
            //    bytes = stream.GetBuffer();
            //    stream.Close();
            //}

            //return bytes;

            MemoryStream stream = SerializeToMemoryStream(obj);
            byte[] bytes = stream.ToArray();
            stream.Close();

            return bytes;
        }
        public MemoryStream SerializeToMemoryStream(object obj)
        {
            MemoryStream stream = null;

            if (obj != null /*&& obj.GetType().IsSerializable*/)
            {
                Type type = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(type);
                stream = new MemoryStream();
                XmlWriter w = new XmlTextWriter(stream, Encoding.UTF8); 
                serializer.Serialize(w, obj); 

                //serializer.Serialize(stream, obj);
            }

            return stream;
        }

        public void SerializeToStream(object obj, Stream stream)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(type);
                XmlWriter w = new XmlTextWriter(stream, Encoding.UTF8);
                serializer.Serialize(w, obj); 
                //serializer.Serialize(stream, obj);
            }
        }

        public object Deserialize(Type type, Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(type);
          
            return serializer.Deserialize(stream);
        }

        public object Deserialize(string str, Encoding encode)
        {
            object msg = null;
            string typeName = PeekTypeName(str, encode);
            if (typeName != null)
            {
                Type type = GetType(typeName);
                msg = Deserialize(type, new MemoryStream(encode.GetBytes(str)));
            }

            return msg;
        }

        public object Deserialize(byte[] bytes)
        {
            object msg = null;
            string typeName = PeekTypeName(bytes);
            if (typeName != null)
            {
                Type type = GetType(typeName);
                msg = Deserialize(type, new MemoryStream(bytes));
            }
            return msg;
        }

        public ParseResult DeserializeToParseResult(byte[] bytes)
        {
            ParseResult result = null;
            string typeName = PeekTypeName(bytes);
            if (typeName != null)
            {
                result = new ParseResult();
                result.Name = typeName;

                Type type = GetType(typeName);
                result.Obj = Deserialize(type, new MemoryStream(bytes));
            }
            return result;
        }

        public object Deserialize(Stream stream)
        {
            object msg = null;
            string typeName = PeekTypeName(stream);
            if (typeName != null)
            {
                try
                {
                    Type type = GetType(typeName);
                    msg = Deserialize(type, stream);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return msg;
        }

        protected abstract Type GetType(string typeName);

        public string PeekTypeName(Stream stream)
        {
            string typeName = null;
            if (stream != null)
            {
                XmlReaderSettings setting = new XmlReaderSettings();
                setting.ValidationType = ValidationType.None;
                setting.IgnoreProcessingInstructions = true;
                setting.ProhibitDtd = false;
                XmlReader reader = XmlReader.Create(stream, setting);

                //parse the xml.  if they exist, display the prefix and  
                //namespace uri of each element.
                //while (reader.Read())
                if (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Prefix == string.Empty)
                        {
                            typeName = TranslateTypeName(reader.LocalName);
                            Console.WriteLine("<{0}>", typeName);
                        }
                        else
                        {
                            //Console.WriteLine("<{0}:{1}>", reader.Prefix, reader.LocalName);
                            //Console.WriteLine(" the namespace uri is " + reader.NamespaceURI);

                            throw new Exception("Invalid xml type definition");
                        }
                        // break;
                    }

                    // reset stream pointer.
                    stream.Seek(0, SeekOrigin.Begin);
                }
            }
            return typeName;
        }

        public string PeekTypeName(byte[] bytes)
        {
            string typeName = null;
            if (bytes != null)
            {
                MemoryStream stream = new MemoryStream(bytes);
                typeName = PeekTypeName(stream);
            }

            return typeName;
        }

        public string PeekTypeName(string str, Encoding encode)
        {
            string typeName = null;
            if (str != null)
            {
                MemoryStream stream = new MemoryStream(encode.GetBytes(str));
                typeName = PeekTypeName(stream);
            }

            return typeName;
        }

        public string TranslateTypeName(string inName)
        {
            string outName = null;

            if (inName != null)
            {
                outName = NameSpacePrefix + inName;
            }

            return outName;
        }
    }
}
