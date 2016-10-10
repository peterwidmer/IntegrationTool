using IntegrationTool.DBAccess;
using IntegrationTool.DBAccess.DatabaseTargetProviders;
using IntegrationTool.DBAccess.DatabaseTargets;
using IntegrationTool.Module.DbCommons;
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

            DataStoreConverter dataStoreConverter = new DataStoreConverter(this.Configuration.TargetTable, dataObject.Metadata, this.Configuration.Mapping);
            
            reportProgress(new SimpleProgressReport("Write records to database"));
            for (int i = 0; i < dataObject.Count; i++)
            {
                object[] rowData = dataObject[i];
                var dbRecord = dataStoreConverter.ConvertToDbRecord(rowData);
                dbRecord.Identifiers= DbTargetHelper.BuildRecordIdentifiers((DbTargetCommonConfiguration)Configuration, rowData, dataObject.Metadata);

                var existingRecords = mySqlTarget.ResolveRecordInDatabase(dbRecord);

                UpsertRecordInDatabase(mySqlTarget, dbRecord, existingRecords);
            }
        }

        public void UpsertRecordInDatabase(IDatabaseTargetProvider databaseTargetProvider, DbRecord dbRecord, List<Object []> existingRecords)
        {
            if(!existingRecords.Any())
            {
                if (this.Configuration.ImportMode == DbTargetImportMode.Create || this.Configuration.ImportMode == DbTargetImportMode.All)
                {
                    databaseTargetProvider.CreateRecordInDatabase(dbRecord);
                }
            }
            else
            {
                if (this.Configuration.ImportMode == DbTargetImportMode.Update || this.Configuration.ImportMode == DbTargetImportMode.All)
                {
                    databaseTargetProvider.UpdateRecordInDatabase(dbRecord);
                }
            }
        }

        
    }
}
