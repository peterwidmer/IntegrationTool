using IntegrationTool.DBAccess.DatabaseTargetProviders;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess.DatabaseTargets
{
    public class DbTargetWriter
    {
        private IDatabaseTargetProvider databaseTargetProvider;
        private IDatastore datastore;
        private DataStoreConverter dataStoreConverter;
        private DbTargetCommonConfiguration configuration;

        public DbTargetWriter(IDatabaseTargetProvider databaseTargetProvider, IDatastore datastore, DataStoreConverter dataStoreConverter, DbTargetCommonConfiguration configuration)
        {
            this.databaseTargetProvider = databaseTargetProvider;
            this.datastore = datastore;
            this.dataStoreConverter = dataStoreConverter;
            this.configuration = configuration;
        }

        public void WriteDataToTarget()
        {
            for (int i = 0; i < datastore.Count; i++)
            {
                object[] rowData = datastore[i];
                var dbRecord = dataStoreConverter.ConvertToDbRecord(rowData);
                dbRecord.Identifiers = DbTargetHelper.BuildRecordIdentifiers(configuration, rowData, datastore.Metadata);

                var existingRecords = databaseTargetProvider.ResolveRecordInDatabase(dbRecord);

                UpsertRecordInDatabase(configuration.ImportMode, dbRecord, existingRecords);
            }
        }

        private void UpsertRecordInDatabase(DbTargetImportMode importMode, DbRecord dbRecord, List<Object[]> existingRecords)
        {
            if (!existingRecords.Any())
            {
                if (importMode == DbTargetImportMode.Create || importMode == DbTargetImportMode.All)
                {
                    databaseTargetProvider.CreateRecordInDatabase(dbRecord);
                }
            }
            else
            {
                if (importMode == DbTargetImportMode.Update || importMode == DbTargetImportMode.All)
                {
                    databaseTargetProvider.UpdateRecordInDatabase(dbRecord);
                }
            }
        }
    }
}
