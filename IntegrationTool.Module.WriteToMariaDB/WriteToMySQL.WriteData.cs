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
                object[] data = dataObject[i];
            }

        }
    }
}
