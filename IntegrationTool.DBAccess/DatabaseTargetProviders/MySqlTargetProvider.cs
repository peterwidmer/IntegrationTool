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

        public void CreateRecordInDatabase(DbRecord dbRecord)
        {
            string createQuery = BuildCreateString(dbRecord);
            var odbcParameters = dbRecord.Values.Select(t => new OdbcParameter(t.Key, t.Value)).ToArray();

            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                odbcWrapper.ExecuteNonQuery(createQuery, odbcParameters);
            }
        }

        private string BuildCreateString(DbRecord dbRecord)
        {
            string columnsToInsert = string.Join(",", dbRecord.Values.Select(t => t.Key));
            string valuePlaceholders = string.Join(",", dbRecord.Values.Select(t => "?"));
            return
                "INSERT INTO "
                + dbRecord.TableName
                + " (" + columnsToInsert + ") values"
                + " (" + valuePlaceholders + ")";
        }

        public void UpdateRecordInDatabase(DbRecord dbRecord, KeyValuePair<string, object>[] recordIdentifiers)
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
