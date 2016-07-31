using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class OdbcWrapper : IDisposable
    {
        private OdbcConnection odbcConnection;
        public string DatabaseName { 
            get
            {
                return odbcConnection.Database;
            }
        }

        public OdbcWrapper(OdbcConnection odbcConnection)
        {
            this.odbcConnection = odbcConnection;
            if(odbcConnection.State != System.Data.ConnectionState.Open)
            {
                odbcConnection.Open();
            }
        }

        public OdbcDataReader ExecuteQuery(string sqlQuery, OdbcParameter [] parameters = null)
        {
            OdbcCommand odbcCommand = new OdbcCommand(sqlQuery, this.odbcConnection);
            if (parameters != null)
            {
                odbcCommand.Parameters.AddRange(parameters);
            }
            OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();

            return odbcDataReader;
        }

        public int ExecuteNonQuery(string sqlQuery, OdbcParameter [] parameters = null)
        {
            OdbcCommand odbcCommand = new OdbcCommand(sqlQuery, this.odbcConnection);
            if (parameters != null)
            {
                odbcCommand.Parameters.AddRange(parameters);
            }
            int result = odbcCommand.ExecuteNonQuery();
            return result;
        }

        public object [] ReadCurrentRow(OdbcDataReader reader)
        {
            object[] row = new object[reader.FieldCount];
            for (int dataIndex = 0; dataIndex < reader.FieldCount; dataIndex++)
            {
                row[dataIndex] = reader.GetValue(dataIndex);
            }

            return row;
        }

        public void Dispose()
        {
            this.odbcConnection.Dispose();
        }
    }
}
