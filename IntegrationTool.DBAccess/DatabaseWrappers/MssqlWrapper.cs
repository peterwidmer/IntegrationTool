using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class MssqlWrapper : IDisposable
    {
        private SqlConnection dbConnection;
        public string DatabaseName { 
            get
            {
                return dbConnection.Database;
            }
        }

        public MssqlWrapper(SqlConnection sqlConnection)
        {
            this.dbConnection = sqlConnection;
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                sqlConnection.Open();
            }
        }

        public SqlDataReader ExecuteQuery(string sqlQuery, SqlParameter[] parameters = null)
        {
            SqlCommand sqlCommand = new SqlCommand(sqlQuery, this.dbConnection);
            if (parameters != null)
            {
                sqlCommand.Parameters.AddRange(parameters);
            }
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            return sqlDataReader;
        }

        public void Dispose()
        {
            this.dbConnection.Dispose();
        }
    }
}
