using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Database
{
    public interface IDatabaseInterface
    {
        void ExecuteNonQuery(string sqlCommand);
        DbDataReader ExecuteQuery(string sqlCommand);
        object ExecuteScalar(string sqlCommand);
        string GetDatabaseName();
        void Dispose();
    }
}
