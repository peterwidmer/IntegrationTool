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

        public List<object[]> ResolveRecordInDatabase(string tableName, KeyValuePair<string, object> [] recordIdentifiers)
        {
            List<object[]> result = new List<object[]>();

            string query = BuildSqlQuery(tableName, recordIdentifiers);
            var odbcParameters= recordIdentifiers.Select(t=> new OdbcParameter(t.Key, t.Value)).ToArray();

            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                var odbcReader = odbcWrapper.ExecuteQuery(query, odbcParameters);
                while (odbcReader.Read())
                {
                    var row = odbcWrapper.ReadCurrentRow(odbcReader);
                    result.Add(row);
                }
            }

            return result;
        }

        private string BuildSqlQuery(string tableName, KeyValuePair<string, object> [] recordIdentifiers)
        {
            string query = "SELECT * FROM " + tableName + " WHERE 1=1";
            foreach (var recordIdentifier in recordIdentifiers)
            {
                query += " AND " + recordIdentifier.Key + "=?";
            }

            return query;
        }
    }
}
