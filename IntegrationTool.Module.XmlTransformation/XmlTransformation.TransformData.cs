using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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

        public static void TransformToDatastore(IDatastore datastore)
        {
            if (datastore.Count <= 0) { return; }

            int columnIndex = 0;

            // Transform xml to table?
            if (Regex.IsMatch(datastore[0][columnIndex].ToString(), @"<\?xml.*\?>.*\n<it_table>"))
            {
                if (datastore[0].Length > 1) { throw new Exception("If data should be transformed to table, you'll allowed to have only one column in the inputdata!"); }
                int rowCount = datastore.Count; // Rowcount increases while adding data, therefore it must be fixed here!
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    XDocument xDocument = XDocument.Parse(datastore[rowIndex][columnIndex].ToString(), LoadOptions.PreserveWhitespace);

                    foreach (var row in xDocument.Root.Elements("it_row"))
                    {
                        AddDatarowToDatastore(datastore, row);
                    }
                }

                datastore.RemoveColumn(datastore.Metadata.Columns.Where(t=> t.Value.ColumnIndex == columnIndex).First().Value.ColumnName);            
            }
        }

        public static void AddDatarowToDatastore(IDatastore datastore, XElement row)
        {
            datastore.AddData(new object[datastore.Metadata.Columns.Count]);
            foreach(var column in row.Elements())
            {
                if(datastore.Metadata.Columns.ContainsKey(column.Name.LocalName) == false)
                {
                    datastore.AddColumn(new ColumnMetadata(datastore.Metadata.Columns.Count, column.Name.LocalName));
                }

                datastore[datastore.Count - 1][datastore.Metadata[column.Name.LocalName].ColumnIndex] = column.Value;
            }
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
