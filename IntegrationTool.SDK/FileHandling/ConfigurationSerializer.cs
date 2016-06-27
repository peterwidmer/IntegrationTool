using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
namespace IntegrationTool.ApplicationCore.Serialization
{
    public class ConfigurationSerializer
    {
        public static string SerializeObject(object obj, Type [] extraTypes)
        {
            System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), extraTypes);
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                serializer.Serialize(ms, obj);
                ms.Position = 0;
                xmlDoc.Load(ms);
                return xmlDoc.OuterXml;
            }
        }

        public static object DeserializeObject(string objectToDeserialize, Type objectType, Type [] extraTypes)
        {
            XmlSerializer serializer = new XmlSerializer(objectType, extraTypes);

            using (var reader = XmlReader.Create(new StringReader(objectToDeserialize), new XmlReaderSettings() { IgnoreWhitespace=false }))
            {
                try
                {
                    return serializer.Deserialize(reader);
                }
                catch
                {
                    // TODO Decide what to do in this case
                    throw;
                }
            }
            
        }
    }
}
