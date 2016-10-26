using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class MySQLDbMetadataProvider : IDatabaseMetadataProvider
    {
        private IConnection connection;


        public MySQLDbMetadataProvider(IConnection connection)
        {
            this.connection = connection;
        }

        public void Initialize()
        {
            this.DatabaseTables = GetDatabaseTables();
        }

        public List<DbMetadataTable> DatabaseTables { get; set; }

        private List<DbMetadataTable> GetDatabaseTables()
        {
            DataTable dt = null;
            List<DbMetadataTable> tables = new List<DbMetadataTable>();
            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                var dataReader = odbcWrapper.ExecuteQuery("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where table_schema=?", new OdbcParameter[] { new OdbcParameter("@dbname", odbcWrapper.DatabaseName) });

                dt = DatabaseHelper.ConvertDataReaderToDataTable(dataReader);                
            }

            foreach (DataRow dr in dt.Rows)
            {
                string tableName = dr["TABLE_NAME"].ToString();
                tables.Add(
                    new DbMetadataTable()
                    {
                        TableName = tableName,
                        Columns = GetTableColumns(tableName)
                    });

            }

            return tables;
        }

        public List<DbMetadataColumn> GetTableColumns(string tableName)
        {
            List<DbMetadataColumn> columns = new List<DbMetadataColumn>();
            using (OdbcWrapper odbcWrapper = new OdbcWrapper(connection.GetConnection() as OdbcConnection))
            {
                var dataReader = odbcWrapper.ExecuteQuery("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = ? AND TABLE_NAME = ?", 
                    new OdbcParameter[] 
                    { 
                        new OdbcParameter("@dbname", odbcWrapper.DatabaseName),
                        new OdbcParameter("@tablename", tableName)
                    });

                DataTable dt = DatabaseHelper.ConvertDataReaderToDataTable(dataReader);
                foreach(DataRow dr in dt.Rows)
                {
                    columns.Add(new DbMetadataColumn() { ColumnName = dr["COLUMN_NAME"].ToString() });
                }
            }

            return columns;
            
        }

        
    }
}
