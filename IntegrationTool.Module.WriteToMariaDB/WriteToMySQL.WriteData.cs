using IntegrationTool.SDK;
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
        public void WriteData(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, SDK.Database.IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            OdbcConnection odbcConnection = (OdbcConnection)connection.GetConnection();
        }
    }
}
