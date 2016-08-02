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

        public List<object[]> ResolveRecordInDatabase(DbRecord dbRecord)
        {
            List<object[]> result = new List<object[]>();

            string query = BuildResolverQuery(dbRecord.TableName, dbRecord.Identifiers);
            var odbcParameters = dbRecord.Identifiers.Select(t => new OdbcParameter(t.Key, t.Value)).ToArray();

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

        private string BuildResolverQuery(string tableName, List<KeyValuePair<string, object>> recordIdentifiers)
        {
            return "SELECT * FROM " + tableName + BuildRecordIdentifierClause(recordIdentifiers);
        }

        public void CreateRecordInDatabase(DbRecord dbRecord)
        {
            string createQuery = BuildCreateString(dbRecord);
            var odbcColumnParameters = dbRecord.Values.Select(t => new OdbcParameter(t.Key, t.Value)).ToArray();

            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                odbcWrapper.ExecuteNonQuery(createQuery, odbcColumnParameters);
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

        public void UpdateRecordInDatabase(DbRecord dbRecord)
        {
            string updateQuery = BuildUpdateString(dbRecord);
            var odbcColumnParameters = dbRecord.Values.Select(t => new OdbcParameter(t.Key, t.Value)).ToArray();
            var odbcWhereParameters = dbRecord.Identifiers.Select(t => new OdbcParameter(t.Key, t.Value)).ToArray();

            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                odbcWrapper.ExecuteNonQuery(updateQuery, odbcColumnParameters.Concat(odbcWhereParameters).ToArray());
            }
        }

        private string BuildUpdateString(DbRecord dbRecord)
        {
            string columnsToUpdate = string.Join(",", dbRecord.Values.Select(t => t.Key + "=?"));
            string whereClause = BuildRecordIdentifierClause(dbRecord.Identifiers);
            return
                "UPDATE "
                + dbRecord.TableName +
                " SET " + columnsToUpdate
                + whereClause;
        }

        private string BuildRecordIdentifierClause(List<KeyValuePair<string, object>> recordIdentifiers)
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
