using IntegrationTool.DBAccess.DatabaseTargetProviders;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToMySql
{
    public partial class WriteToMySQL
    {
        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Aquire database-connection"));
            OdbcConnection odbcConnection = (OdbcConnection)connection.GetConnection();

            reportProgress(new SimpleProgressReport("Write records to database"));
            for (int i = 0; i < dataObject.Count; i++)
            {
                object[] rowData = dataObject[i];

                var recordIdentifiers = BuildRecordIdentifiers(rowData, dataObject.Metadata);
                MySqlTargetProvider mySqlTarget = new MySqlTargetProvider(connection);
                var existingRecords = mySqlTarget.ResolveRecordInDatabase(Configuration.TargetTable, recordIdentifiers);
                
            }
        }

        public KeyValuePair<string, object> [] BuildRecordIdentifiers(object[] rowData, DataMetadata dataMetadata)
        {
            List<KeyValuePair<string, object>> recordIdentifiers = new List<KeyValuePair<string, object>>();

            foreach(var primaryKeyField in Configuration.PrimaryKeyFields)
            {
                var primaryKeyMapping = Configuration.Mapping.FirstOrDefault(t => t.Target == primaryKeyField);
                if(primaryKeyMapping == null)
                {
                    throw new Exception("The primarykey " + primaryKeyField + " is not mapped!");
                }

                var recordIdentifier = new KeyValuePair<string, object>(primaryKeyField, rowData[dataMetadata[primaryKeyMapping.Source].ColumnIndex]);
                recordIdentifiers.Add(recordIdentifier);
            }

            return recordIdentifiers.ToArray();
        }
    }
}
