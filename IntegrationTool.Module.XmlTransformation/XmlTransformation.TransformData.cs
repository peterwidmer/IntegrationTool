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
    public class TransformationLog
    {
        public List<string> NewColumns { get; set; }
        public List<int> RowNumbersToHide { get; set; }

        public TransformationLog()
        {
            this.NewColumns = new List<string>();
            this.RowNumbersToHide = new List<int>();
        }    
    }

    public partial class XmlTransformation
    {
        public void TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            TransformToDatastore(dataObject, this.Configuration.TransformationXslt, this.Configuration.InputXmlColumn, false);           
        }

        public static TransformationLog TransformToDatastore(IDatastore datastore, string xslTransformation, string columnToTransform, bool isRunFromPreview)
        {
            TransformationLog transformationLog = new TransformationLog();

            if (datastore.Count <= 0) { return transformationLog; }

            int columnIndex = datastore.Metadata.Columns[columnToTransform].ColumnIndex;
            int rowCount = datastore.Count; // Rowcount increases while adding data, therefore it must be fixed here!

            // Transform xml to table?
            string transformedXml = TransformXml(datastore[0][columnIndex].ToString(), xslTransformation);
            if (Regex.IsMatch(transformedXml, @"<\?xml.*\?>.*\n<it_table>"))
            {                
                for (int sourceRowIndex = 0; sourceRowIndex < rowCount; sourceRowIndex++)
                {
                    transformedXml = TransformXml(datastore[sourceRowIndex][columnIndex].ToString(), xslTransformation);
                    XDocument xDocument = XDocument.Parse(transformedXml, LoadOptions.PreserveWhitespace);

                    foreach (var row in xDocument.Root.Elements("it_row"))
                    {
                        AddDatarowToDatastore(datastore, transformationLog, sourceRowIndex, row);
                    }
                }

                // Remove transformed rows
                if (isRunFromPreview == false)
                {
                    for (int rowIndex = rowCount - 1; rowIndex >= 0; rowIndex--)
                    {
                        datastore.RemoveDataAt(rowIndex);
                    }
                    datastore.RemoveColumn(datastore.Metadata.Columns.Where(t => t.Value.ColumnIndex == columnIndex).First().Value.ColumnName);
                }
                else
                {
                    for (int rowIndex = rowCount - 1; rowIndex >= 0; rowIndex--)
                    {
                        transformationLog.RowNumbersToHide.Add(rowIndex);
                    }
                }
            }
            else
            {
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    datastore.SetValue(rowIndex, columnIndex, TransformXml(datastore[rowIndex][columnIndex].ToString(), xslTransformation));
                }
            }

            return transformationLog;
        }

        public static void AddDatarowToDatastore(IDatastore datastore, TransformationLog transformationLog, int sourceRowIndex, XElement row)
        {
            datastore.AddData(new object[datastore.Metadata.Columns.Count]);

            // Copy values from sourcerow to new row
            for(int i=0; i < datastore[sourceRowIndex].Length; i++)
            {
                datastore[datastore.Count - 1][i] = datastore[sourceRowIndex][i];
            }

            // Add new columns if necessary and fill the values
            foreach(var column in row.Elements())
            {
                if(datastore.Metadata.Columns.ContainsKey(column.Name.LocalName) == false)
                {
                    transformationLog.NewColumns.Add(column.Name.LocalName);
                    datastore.AddColumn(new ColumnMetadata(column.Name.LocalName));
                }

                datastore.SetValue(datastore.Count - 1, datastore.Metadata[column.Name.LocalName].ColumnIndex, column.Value);
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
