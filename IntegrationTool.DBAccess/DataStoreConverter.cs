using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DataStoreConverter
    {
        private string targetTable;
        private DataMetadata dataMetadata;
        private List<DataMapping> dataMapping;

        public DataStoreConverter(string targetTable, DataMetadata dataMetadata, List<DataMapping> dataMapping)
        {
            this.targetTable = targetTable;
            this.dataMetadata = dataMetadata;
            this.dataMapping = dataMapping;
        }

        public DbRecord ConvertToDbRecord(object[] rowData)
        {
            var dbRecord = new DbRecord(targetTable);
            foreach (var mapping in dataMapping)
            {
                int sourceColumnIndex = dataMetadata[mapping.Source].ColumnIndex;
                dbRecord.Values.Add(new KeyValuePair<string, object>(mapping.Target, rowData[sourceColumnIndex]));
            }

            return dbRecord;
        }
    }
}
