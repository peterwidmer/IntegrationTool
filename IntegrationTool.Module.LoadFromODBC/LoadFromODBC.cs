using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromODBC
{
    [SourceModuleAttribute(Name = "LoadFromODBC",
                           DisplayName = "ODBC",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(OdbcConnection),
                           ConfigurationType = typeof(LoadFromODBCConfiguration))]
    public class LoadFromODBC : IModule, IDataSource
    {
        public LoadFromODBCConfiguration Configuration { get; set; }

        public LoadFromODBC()
        {
            Configuration = new LoadFromODBCConfiguration();
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromODBCConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromODBCConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress)
        {
            OdbcConnection odbcConnection = connection.GetConnection() as OdbcConnection;
            if(odbcConnection.State != System.Data.ConnectionState.Open)
            {
                odbcConnection.Open();
            }

            OdbcCommand odbcCommand = new OdbcCommand(Configuration.SqlValue, odbcConnection);
            OdbcDataReader reader = odbcCommand.ExecuteReader();

            int counter = 0;
            while(reader.Read())
            {
                // Read the metadata from the source
                if(datastore.Count == 0)
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        datastore.AddColumnMetadata(new ColumnMetadata(i, reader.GetName(i)));
                    }
                }

                // Add data to the datastore
                object[] data = new object[reader.FieldCount];
                for(int dataIndex = 0; dataIndex < reader.FieldCount; dataIndex++)
                {
                    data[dataIndex] = reader.GetValue(dataIndex);
                }           
                datastore.AddData(data);

                if (StatusHelper.MustShowProgress(counter, -1) == true)
                {
                    reportProgress(new SimpleProgressReport("Loaded " + (counter + 1) + " records"));
                }

                counter++;
            }
            odbcConnection.Dispose();
        }
    }
}
