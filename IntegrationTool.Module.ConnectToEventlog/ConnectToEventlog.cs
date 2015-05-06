using IntegrationTool.SDK;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToEventlog
{
    [ConnectionModuleAttribute(
        DisplayName = "Eventlog",
        Name = "Eventlog",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(EventLog),
        ConfigurationType = typeof(ConnectToEventlogConfiguration))]
    public class ConnectToEventlog : IModule, IConnection
    {
        public ConnectToEventlogConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToEventlogConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToEventlogConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            if (!EventLog.SourceExists(this.Configuration.Source) && !EventLog.Exists(this.Configuration.LogName))
            {               
                EventLog.CreateEventSource(this.Configuration.Source, this.Configuration.LogName);
                Thread.Sleep(250); // When an eventlog is created, it is not immediatly available
            }

            return new string[] { this.Configuration.Source, this.Configuration.LogName };
        }
    }
}
