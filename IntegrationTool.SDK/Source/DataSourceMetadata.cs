using IntegrationTool.SDK.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class DataMetadata
    {
        public List<ColumnMetadata> Columns { get; set; }
        public DataMetadata()
        {
            Columns = new List<ColumnMetadata>();
        }

        public List<NameDisplayName> GetColumnsAsNameDisplayNameList()
        {
            List<NameDisplayName> columnList = new List<NameDisplayName>();
            foreach (ColumnMetadata column in this.Columns)
            {
                columnList.Add(new NameDisplayName(column.ColumnName, column.ColumnName));
            }

            return columnList;
        }

        public Dictionary<string, ColumnMetadata> GetColumnsDictionary()
        {
            Dictionary<string, ColumnMetadata> dictionary = new Dictionary<string, ColumnMetadata>();
            foreach (ColumnMetadata column in this.Columns)
            {
                dictionary.Add(column.ColumnName, column);
            }

            return dictionary;
        }

        public ColumnMetadata this[string ColumnName]
        {
            get { return this.Columns.Where(t => t.ColumnName == ColumnName).FirstOrDefault(); }
        }
    }
}
