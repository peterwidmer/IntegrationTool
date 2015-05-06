using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DummyDatabaseInterface : IDatabaseInterface
    {
        public void ExecuteNonQuery(string sqlCommand)
        {
        }

        public System.Data.Common.DbDataReader ExecuteQuery(string sqlCommand)
        {
            return null;
        }

        public object ExecuteScalar(string sqlCommand)
        {
            return null;
        }

        public string GetDatabaseName()
        {
            return "none";
        }

        public void Dispose()
        {
        }
    }
}
