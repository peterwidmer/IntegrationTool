using IntegrationTool.SDK;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToSQLite
{
    [ConnectionModuleAttribute(
        DisplayName = "SQLite",
        Name = "ConnectToSQLite",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(SQLiteConnection),
        ConfigurationType = typeof(ConnectToSQLiteConfiguration))]
    public class ConnectToSQLite : IModule, IConnection
    {
        public ConnectToSQLiteConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToSQLiteConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToSQLiteConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            return new SQLiteConnection(this.Configuration.ConnectionString);
        }
    }
}
