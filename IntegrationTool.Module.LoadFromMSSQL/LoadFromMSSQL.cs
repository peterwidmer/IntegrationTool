using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromMSSQL
{
    [SourceModuleAttribute(Name = "LoadFromMSSQL",
                           DisplayName = "MSSQL",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(SqlConnection),
                           ConfigurationType = typeof(LoadFromMSSQLConfiguration))]
    public class LoadFromMSSQL: IModule, IDataSource
    {
        public LoadFromMSSQLConfiguration Configuration { get; set; }

        public LoadFromMSSQL()
        {
            Configuration = new LoadFromMSSQLConfiguration();
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromMSSQLConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress, bool mappingPreview)
        {
            using (SqlConnection sqlConnection = connection.GetConnection() as SqlConnection)
            {
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(Configuration.SqlValue, sqlConnection))
                {
                    using (SqlDataReader reader = sqlCommand.ExecuteReader())
                    {
                        int counter = 0;
                        while (reader.Read())
                        {
                            // Read the metadata from the source
                            if (datastore.Count == 0)
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    datastore.AddColumn(new ColumnMetadata(reader.GetName(i)));
                                }
                            }

                            // Add data to the datastore
                            object[] data = new object[reader.FieldCount];
                            for (int dataIndex = 0; dataIndex < reader.FieldCount; dataIndex++)
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
                    }
                }
            }
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromMSSQLConfiguration;
        }
    }
}
