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

            DbTargetWriter dbTargetWriter = new DbTargetWriter(mySqlTarget, dataObject, dataStoreConverter, this.Configuration);
            dbTargetWriter.WriteDataToTarget();
        }        
    }
}
