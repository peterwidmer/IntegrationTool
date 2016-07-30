using IntegrationTool.DBAccess.DatabaseTargetProviders;
using IntegrationTool.Module.WriteToMySQL.SDK.Enums;
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
            IDatabaseTargetProvider mySqlTarget = new MySqlTargetProvider(connection);

            reportProgress(new SimpleProgressReport("Write records to database"));
            for (int i = 0; i < dataObject.Count; i++)
            {
                object[] rowData = dataObject[i];
                var recordIdentifiers = BuildRecordIdentifiers(rowData, dataObject.Metadata);
                var existingRecords = mySqlTarget.ResolveRecordInDatabase(Configuration.TargetTable, recordIdentifiers);
            }
        }

        public void UpsertRecordInDatabase(IDatabaseTargetProvider databaseTargetProvider, object[] rowData, List<Object []> existingRecords)
        {
            if(existingRecords.Count > 0)
            {
                if(this.Configuration.ImportMode == MySqlImportMode.Create || this.Configuration.ImportMode == MySqlImportMode.All)
                {
                    databaseTargetProvider.CreateRecordInDatabase(this.Configuration.TargetTable, rowData, this.Configuration.Mapping);
                }
            }
            else
            {

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
