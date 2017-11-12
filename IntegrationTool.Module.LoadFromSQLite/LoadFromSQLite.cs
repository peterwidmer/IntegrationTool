using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromSQLite
{
    [SourceModuleAttribute(Name = "LoadFromSQLite",
                           DisplayName = "SQLite",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(SQLiteConnection),
                           ConfigurationType = typeof(LoadFromSQLiteConfiguration))]
    public class LoadFromSQLite : IModule, IDataSource
    {
        public LoadFromSQLiteConfiguration Configuration { get; set; }

        public LoadFromSQLite()
        {
            Configuration = new LoadFromSQLiteConfiguration();
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromSQLiteConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromSQLiteConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress)
        {
            var databaseConnection = connection.GetConnection() as SQLiteConnection;
            if (databaseConnection.State != System.Data.ConnectionState.Open)
            {
                databaseConnection.Open();
            }

            var databaseCommand = new SQLiteCommand(Configuration.SqlValue, databaseConnection);
            var databaseReader = databaseCommand.ExecuteReader();

            int counter = 0;
            while (databaseReader.Read())
            {
                // Read the metadata from the source
                if (datastore.Count == 0)
                {
                    for (int i = 0; i < databaseReader.FieldCount; i++)
                    {
                        datastore.AddColumn(new ColumnMetadata(databaseReader.GetName(i)));
                    }
                }

                // Add data to the datastore
                object[] data = new object[databaseReader.FieldCount];
                for (int dataIndex = 0; dataIndex < databaseReader.FieldCount; dataIndex++)
                {
                    data[dataIndex] = databaseReader.GetValue(dataIndex);
                }
                datastore.AddData(data);

                if (StatusHelper.MustShowProgress(counter, -1) == true)
                {
                    reportProgress(new SimpleProgressReport("Loaded " + (counter + 1) + " records"));
                }

                counter++;
            }
            databaseConnection.Dispose();
        }
    }
}
