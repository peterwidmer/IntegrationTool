using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class SqliteWrapper : IDatabaseInterface
    {
        private System.Data.Common.DbConnection dbConnection;
        private string dataBasePath;
        private string dataBaseName;

        public SqliteWrapper(string directory, string databaseName)
        {
            this.dataBaseName = databaseName;
            this.dataBasePath = BuildDataBasePath(directory, databaseName);
            
            dbConnection = GetDatabase();
            dbConnection.Open();
        }

        private System.Data.Common.DbConnection GetDatabase()
        {
            if (File.Exists(this.dataBasePath) == false)
            {
                SQLiteConnection.CreateFile(this.dataBasePath);
            }

            string dataBaseConnectionString = GetSQLiteConnectionString(this.dataBasePath);
            SQLiteConnection connection = new SQLiteConnection(dataBaseConnectionString);

            return connection;
        }

        public void DeleteDatabase()
        {
            if(File.Exists(this.dataBasePath))
            {
                File.Delete(this.dataBasePath);
            }
        }

        public void ExecuteNonQuery(string sqlCommand)
        {
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.dbConnection as SQLiteConnection);
            command.ExecuteNonQuery();
        }

        public System.Data.Common.DbDataReader ExecuteQuery(string sqlCommand)
        {
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.dbConnection as SQLiteConnection);
            SQLiteDataReader reader = command.ExecuteReader();

            return reader;
        }

        public object ExecuteScalar(string sqlCommand)
        {
            SQLiteCommand command = new SQLiteCommand(sqlCommand, this.dbConnection as SQLiteConnection);
            object result = command.ExecuteScalar();

            return result;
        }

        private string GetSQLiteConnectionString(string dataBasePath)
        {
            return "Data Source=" + dataBasePath + ";Version=3;UseUTF16Encoding=True;";
        }

        private string BuildDataBasePath(string directory, string databaseName)
        {
            directory += (directory.EndsWith(@"\") == false) ? @"\" : "";
            string dataBasePath = directory + databaseName;

            return dataBasePath;
        }

        public string GetDatabaseName()
        {
            return this.dataBaseName;
        }

        public static SqliteWrapper GetSqliteWrapper(string runLogPath, Guid itemId, string itemLabel)
        {
            string databaseName = itemId + "_" + itemLabel.Replace(" ", "_") + ".db";
            SqliteWrapper sqliteWrapper = new SqliteWrapper(runLogPath, databaseName);

            return sqliteWrapper;
        }

        public void Dispose()
        {
            this.dbConnection.Dispose();
        }


        
    }
}
