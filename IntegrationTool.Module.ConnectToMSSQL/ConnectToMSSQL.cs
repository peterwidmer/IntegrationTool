using IntegrationTool.SDK;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToMSSQL
{
    [ConnectionModuleAttribute(
    DisplayName = "MSSQL",
    Name = "ConnectToMSSQL",
    ContainsSubConfiguration = false,
    ModuleType = ModuleType.Connection,
    ConnectionType = typeof(SqlConnection),
    ConfigurationType = typeof(ConnectToMSSQLConfiguration))]
    public class ConnectToMSSQL : IModule, IConnection
    {
        public ConnectToMSSQLConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToMSSQLConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(this.Configuration.ConnectionString);
            return sqlConnection;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToMSSQLConfiguration;
        }
    }
}
