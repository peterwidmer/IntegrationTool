using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.Module.ConnectToUrl
{
    [ConnectionModuleAttribute(
        DisplayName = "Url",
        Name = "ConnectToUrl",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(ConnectToUrlConfiguration),
        ConfigurationType = typeof(ConnectToUrlConfiguration))]
    public class ConnectToUrl : IModule, IConnection
    {
        public ConnectToUrlConfiguration Configuration;

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToUrlConfiguration;
        }

        public UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToUrlConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            return Configuration;
        }
    }
}
