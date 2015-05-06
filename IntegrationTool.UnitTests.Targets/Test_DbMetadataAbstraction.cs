using IntegrationTool.Module.ConnectToODBC;
using IntegrationTool.SDK;
using IntegrationTool.UnitTests.Targets.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using IntegrationTool.DBAccess;
using System.Data;
using System.Data.Common;

namespace IntegrationTool.UnitTests.Targets
{
    [TestClass]
    public class Test_DbMetadataAbstraction
    {
        private static IConnection connection;

        [ClassInitialize]
        public static void Initialize_DdMetadataAbstraction(TestContext context)
        {
            ConnectToODBCConfiguration configuration = new ConnectToODBCConfiguration();
            configuration.ConnectionString = Settings.Default.OdbcConnectionString;

            connection = new ConnectToODBC() { Configuration = configuration };
        }

        [TestMethod]
        public void TestMariaDbConnection()
        {
            OdbcConnection odbcConnection = connection.GetConnection() as OdbcConnection;
            if (odbcConnection.State != System.Data.ConnectionState.Open)
            {
                odbcConnection.Open();
            }

            OdbcCommand odbcCommand = new OdbcCommand("Select * from account", odbcConnection);
            OdbcDataReader reader = odbcCommand.ExecuteReader();

            while (reader.Read())
            {
                // Read the metadata from the source

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string strName = reader.GetValue(i).ToString();
                }
            }
            odbcConnection.Dispose();
        }

        [TestMethod]
        public void TestMariaDbSchema()
        {
            MySQLDbMetadataProvider mariaDbMetadataProvider = new MySQLDbMetadataProvider(connection);
            var tables = mariaDbMetadataProvider.DatabaseTables;

            Assert.IsTrue(tables.Count > 0);
        }
    }
}
