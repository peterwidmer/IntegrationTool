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
        public Dictionary<string, ColumnMetadata> Columns { get; set; }
        public DataMetadata()
        {
            Columns = new Dictionary<string, ColumnMetadata>();
        }

        public List<NameDisplayName> GetColumnsAsNameDisplayNameList()
        {
            List<NameDisplayName> columnList = new List<NameDisplayName>();
            foreach (ColumnMetadata column in this.Columns.Values)
            {
                columnList.Add(new NameDisplayName(column.ColumnName, column.ColumnName));
            }

            return columnList;
        }

        public Dictionary<string, ColumnMetadata> GetColumnsDictionary()
        {
            Dictionary<string, ColumnMetadata> dictionary = new Dictionary<string, ColumnMetadata>();
            foreach (ColumnMetadata column in this.Columns.Values)
            {
                dictionary.Add(column.ColumnName, column);
            }

            return dictionary;
        }

        public ColumnMetadata this[string columnName]
        {
            get { return this.Columns[columnName]; }
        }
    }
}
