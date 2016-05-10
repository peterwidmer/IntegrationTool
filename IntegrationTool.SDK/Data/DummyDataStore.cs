using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data
{
    public class DummyDataStore : IDatastore
    {
        public void InitializeDatastore(List<AttributeImplementation> dataConditionAttributes)
        {
        }

        public int Count
        {
            get { return 0; }
        }

        public DataMetadata Metadata {get; set;}

        public void AddColumn(ColumnMetadata columnMetadata)
        {
        }

        public void RemoveColumn(string columnName)
        {
        }

        public void AddData(object[] data)
        {
        }

        public void RemoveDataAt(int index)
        {
        }

        public void SetValue(int index, int column, object value)
        {
        }

        public void ApplyFilter(DataFilter dataFilter)
        {
        }

        public void ClearFilter()
        {
        }

        public List<GenericClasses.NameDisplayName> GetDistinctValuesOfColumn(string columnName)
        {
            return new List<GenericClasses.NameDisplayName>();
        }

        public object[] this[int rowIndex]
        {
            get { return null; }
        }
    }
}
