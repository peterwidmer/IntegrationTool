using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Database
{
    public interface IDatastore
    {
        void InitializeDatastore(List<AttributeImplementation> dataConditionAttributes);
        int Count { get; }
        DataMetadata Metadata { get; set; }
        void AddColumn(ColumnMetadata columnMetadata);
        void RemoveColumn(string columnName);
        void AddData(object[] data);
        void RemoveDataAt(int index);
        void SetValue(int index, int column, object value);
        void ApplyFilter(DataFilter dataFilter);
        void ClearFilter();
        List<NameDisplayName> GetDistinctValuesOfColumn(string columnName);

        // Indexer declaration: 
        object[] this[int rowIndex] { get; }

    }
}
