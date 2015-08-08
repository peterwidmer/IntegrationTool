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

        private string TransformXml(string inputXml, string xslTransformation)
        {
            XPathDocument doc = new XPathDocument(new StringReader(inputXml));

            StringWriter sw = new StringWriter();

            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings2 = new XsltSettings();
            settings2.EnableScript = true;
            transform.Load(XmlReader.Create(new StringReader(xslTransformation)), settings2, null);
            transform.Transform(doc, null, sw);

            return sw.ToString();
        }
    }
}
