using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;


namespace ActionEditor
{
    [Serializable]
    public class EditorSetupData : ICloneable
    {
        string mResourcePath;
        [DefaultValue(""), DisplayName("客户端资源路径"), XmlElement("ResourcePath")]
        public string ResourcePath { get { return mResourcePath; } set { mResourcePath = value; } }

        string mTablePath;
        [DefaultValue(""), DisplayName("表格路径"), XmlElement("TablePath")]
        public string TablePath { get { return mTablePath; } set { mTablePath = value; } }

        public Object Clone()
        {
            // save current object to xml.
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer serializer = new XmlSerializer(typeof(EditorSetupData));
            serializer.Serialize(memoryStream, this);

            // deserializer out.
            memoryStream.Seek(0, SeekOrigin.Begin);
            EditorSetupData cloneObject = (EditorSetupData)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            // reset copy datas.
            return cloneObject;
        }
    }
}
