using IntegrationTool.DataMappingControl;
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

            string query = BuildResolverQuery(tableName, recordIdentifiers);
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

        private string BuildResolverQuery(string tableName, KeyValuePair<string, object> [] recordIdentifiers)
        {
            return "SELECT * FROM " + tableName + BuildRecordIdentifierClause(recordIdentifiers);
        }

        public void CreateRecordInDatabase(string tableName, object[] recordToCreate, List<DataMapping> dataMapping)
        {
            string createQuery = BuildCreateString(tableName, dataMapping);
            // TODO Resolve values to insert
        }

        private string BuildCreateString(string tableName, List<DataMapping> dataMapping)
        {
            string columnsToInsert = string.Join(",", dataMapping.Select(t => t.Target));
            string valuePlaceholders = string.Join(",", dataMapping.Select(t => "?"));
            return
                "INSERT INTO TABLE "
                + tableName
                + " (" + columnsToInsert + ") values"
                + " (" + valuePlaceholders + ")";
        }

        public void UpdateRecordInDatabase(string tableName, object[] recordToCreate, KeyValuePair<string, object>[] recordIdentifiers)
        {
            throw new NotImplementedException();
        }

        private string BuildRecordIdentifierClause(KeyValuePair<string, object>[] recordIdentifiers)
        {
            var query = new StringBuilder(" WHERE 1=1");
            foreach (var recordIdentifier in recordIdentifiers)
            {
                query.Append(" AND " + recordIdentifier.Key + "=?");
            }

            return query.ToString();
        }
    }
}
