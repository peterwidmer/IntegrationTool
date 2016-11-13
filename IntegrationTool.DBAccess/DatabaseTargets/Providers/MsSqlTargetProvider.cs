using IntegrationTool.DBAccess.DatabaseTargetProviders;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess.DatabaseTargets.Providers
{
    public class MsSqlTargetProvider : IDatabaseTargetProvider
    {
        private IConnection connection;

        public MsSqlTargetProvider(IConnection connection)
        {
            this.connection = connection;
        }

        public List<object[]> ResolveRecordInDatabase(DbRecord dbRecord)
        {
            List<object[]> result = new List<object[]>();

            string query = BuildResolverQuery(dbRecord.TableName, dbRecord.Identifiers);
            var sqlParameters = dbRecord.Identifiers.Select(t => new SqlParameter("@" + t.Key, t.Value)).ToArray();

            using (var sqlWrapper = new MssqlWrapper(connection.GetConnection() as SqlConnection))
            {
                var sqlReader = sqlWrapper.ExecuteQuery(query, sqlParameters);
                while (sqlReader.Read())
                {
                    var row = sqlWrapper.ReadCurrentRow(sqlReader);
                    result.Add(row);
                }
            }

            return result;
        }

        private string BuildResolverQuery(string tableName, List<KeyValuePair<string, object>> recordIdentifiers)
        {
            return "SELECT * FROM " + tableName + BuildRecordIdentifierClause(recordIdentifiers);
        }

        private string BuildRecordIdentifierClause(List<KeyValuePair<string, object>> recordIdentifiers)
        {
            var query = new StringBuilder(" WHERE 1=1");
            foreach (var recordIdentifier in recordIdentifiers)
            {
                query.Append(" AND " + recordIdentifier.Key + "=@" + recordIdentifier.Key);
            }

            return query.ToString();
        }

        public void CreateRecordInDatabase(DbRecord dbRecord)
        {
            string createQuery = BuildCreateString(dbRecord);
            var sqlParameters = dbRecord.Values.Select(t => new SqlParameter("@" + t.Key, t.Value)).ToArray();

            using (var sqlWrapper = new MssqlWrapper(connection.GetConnection() as SqlConnection))
            {
                sqlWrapper.ExecuteNonQuery(createQuery, sqlParameters);
            }
        }

        private string BuildCreateString(DbRecord dbRecord)
        {
            string columnsToInsert = string.Join(",", dbRecord.Values.Select(t => t.Key));
            string valuePlaceholders = string.Join(",", dbRecord.Values.Select(t => "@" + t.Key));
            return
                "INSERT INTO "
                + dbRecord.TableName
                + " (" + columnsToInsert + ") values"
                + " (" + valuePlaceholders + ")";
        }

        public void UpdateRecordInDatabase(DbRecord dbRecord)
        {
            string updateQuery = BuildUpdateString(dbRecord);
            var sqlColumnParameters = dbRecord.Values.Select(t => new SqlParameter("@" + t.Key, t.Value)).ToArray();
            var sqlWhereParameters = dbRecord.Identifiers.Select(t => new SqlParameter("@" + t.Key, t.Value)).ToArray();

            using (var sqlWrapper = new MssqlWrapper(connection.GetConnection() as SqlConnection))
            {
                sqlWrapper.ExecuteNonQuery(updateQuery, sqlColumnParameters.Concat(sqlWhereParameters).ToArray());
            }
        }

        private string BuildUpdateString(DbRecord dbRecord)
        {
            string columnsToUpdate = string.Join(",", dbRecord.Values.Select(t => t.Key + "=@" + t.Key));
            string whereClause = BuildRecordIdentifierClause(dbRecord.Identifiers);
            return
                "UPDATE "
                + dbRecord.TableName +
                " SET " + columnsToUpdate
                + whereClause;
        }
    }
}
