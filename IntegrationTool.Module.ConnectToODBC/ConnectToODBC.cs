using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using IntegrationTool.SDK.Module.ModuleAttributes;

namespace IntegrationTool.Module.ConnectToODBC
{
    [ConnectionModuleAttribute(
        DisplayName = "ODBC",
        Name = "ConnectToODBC",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(OdbcConnection),
        ConfigurationType = typeof(ConnectToODBCConfiguration))]
    public class ConnectToODBC : IModule, IConnection
    {
        public ConnectToODBCConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToODBCConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToODBCConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            OdbcConnection odbcConnection = new OdbcConnection(this.Configuration.ConnectionString);
            return odbcConnection;
        }
    }
}
