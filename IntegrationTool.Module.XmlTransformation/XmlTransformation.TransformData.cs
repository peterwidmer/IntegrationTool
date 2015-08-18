﻿using IntegrationTool.SDK;
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
            TransformToDatastore(dataObject, this.Configuration.TransformationXslt, this.Configuration.InputXmlColumn);           
        }

        public static void TransformToDatastore(IDatastore datastore, string xslTransformation, string columnToTransform)
        {
            if (datastore.Count <= 0) { return; }

            int columnIndex = datastore.Metadata.Columns[columnToTransform].ColumnIndex;
            int rowCount = datastore.Count; // Rowcount increases while adding data, therefore it must be fixed here!

            // Transform xml to table?            
            if (Regex.IsMatch(datastore[0][columnIndex].ToString(), @"<\?xml.*\?>.*\n<it_table>"))
            {
                if (datastore[0].Length > 1) { throw new Exception("If data should be transformed to table, you'll allowed to have only one column in the inputdata!"); }
                
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    string transformedXml = TransformXml(datastore[rowIndex][columnIndex].ToString(), xslTransformation);
                    XDocument xDocument = XDocument.Parse(transformedXml, LoadOptions.PreserveWhitespace);

                    foreach (var row in xDocument.Root.Elements("it_row"))
                    {
                        AddDatarowToDatastore(datastore, row);
                    }
                }

                // Remove transformed rows
                for (int rowIndex = rowCount - 1; rowIndex >= 0; rowIndex--)
                {
                    datastore.RemoveDataAt(rowIndex);
                }

                datastore.RemoveColumn(datastore.Metadata.Columns.Where(t=> t.Value.ColumnIndex == columnIndex).First().Value.ColumnName);            
            }
            else
            {
                for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
                {
                    datastore[rowIndex][columnIndex] = TransformXml(datastore[rowIndex][columnIndex].ToString(), xslTransformation);
                }
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
