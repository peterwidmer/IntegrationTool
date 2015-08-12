using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace IntegrationTool.Module.XmlTransformation
{
    public partial class XmlTransformation
    {
        public void TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            // TODO Do the xml-transformation            
        }

        public static string TransformXml(string inputXml, string xslTransformation)
        {
            XmlDocument source = new XmlDocument();
            StringReader reader = new StringReader(inputXml);
            source.Load(reader);

            XslCompiledTransform transformer = new XslCompiledTransform(false);
            transformer.Load(XmlReader.Create(new StringReader(xslTransformation)));

            string transformedXml = string.Empty;

            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            {
                transformer.Transform(source, null, sw);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    transformedXml = sr.ReadToEnd();
                }
            }

            return transformedXml;
        }
    }
}
