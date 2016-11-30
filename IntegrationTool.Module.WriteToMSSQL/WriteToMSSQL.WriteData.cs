using IntegrationTool.DBAccess;
using IntegrationTool.DBAccess.DatabaseTargetProviders;
using IntegrationTool.DBAccess.DatabaseTargets;
using IntegrationTool.DBAccess.DatabaseTargets.Providers;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToMSSQL
{
    public partial class WriteToMSSQL
    {
        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Aquire database-connection"));
            IDatabaseTargetProvider msSqlTarget = new MsSqlTargetProvider(connection);

            DataStoreConverter dataStoreConverter = new DataStoreConverter(this.Configuration.TargetTable, dataObject.Metadata, this.Configuration.Mapping);

            reportProgress(new SimpleProgressReport("Write records to database"));

            DbTargetWriter dbTargetWriter = new DbTargetWriter(msSqlTarget, dataObject, dataStoreConverter, this.Configuration);
            dbTargetWriter.WriteDataToTarget();
        }

    }
}
