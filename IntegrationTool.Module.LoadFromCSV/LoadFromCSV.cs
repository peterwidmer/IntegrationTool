using CsvHelper;
using IntegrationTool.SDK;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromCSV
{
    [SourceModuleAttribute(Name = "LoadFromCSV",
                           DisplayName = "CSV",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(StringReader),
                           ConfigurationType = typeof(LoadFromCSVConfiguration))]
    public class LoadFromCSV : IModule, IDataSource
    {
        public LoadFromCSVConfiguration Configuration { get; set; }
        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromCSVConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromCSVConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress)
        {
            using (StringReader stringReader = connection.GetConnection() as StringReader)
            {
                var reader = new CsvReader(stringReader);
                reader.Configuration.Delimiter = this.Configuration.Delimiter;
                reader.Configuration.Comment = this.Configuration.Comment[0];
                reader.Configuration.Quote = this.Configuration.Quote[0];
                reader.Configuration.QuoteAllFields = this.Configuration.QuoteAllFields;

                while (reader.Read())
                {
                    // Read the metadata from the source
                    if (datastore.Count == 0)
                    {
                        for (int i = 0; i < reader.FieldHeaders.Length; i++)
                        {
                            datastore.AddColumn(new ColumnMetadata(reader.FieldHeaders[i]));
                        }
                    }

                    // Add data to the datastore
                    object[] data = new object[reader.FieldHeaders.Length];
                    for (int dataIndex = 0; dataIndex < reader.FieldHeaders.Length; dataIndex++)
                    {
                        data[dataIndex] = reader.GetField(dataIndex);
                    }
                    datastore.AddData(data);
                }
            }
        }
    }
}
