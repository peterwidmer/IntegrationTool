using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess.DatabaseTargetProviders
{
    public class MySqlTargetProvider : IDatabaseTargetProvider
    {
        private IConnection connection;

        public MySqlTargetProvider(IConnection connection)
        {
            this.connection = connection;
        }

        public void ResolveRecordInDatabase(string tableName, KeyValuePair<string, object> [] recordIdentifiers)
        {
            string query = "SELECT * FROM " + tableName + " WHERE 1=1";
            foreach(var recordIdentifier in recordIdentifiers)
            {
                query += " AND " + recordIdentifier.Key + "=?"; 
            }

            // NEXT TODO -> Add parameter-values to command

            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                
            }
        }
    }
}
