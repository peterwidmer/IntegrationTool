using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Module.ModuleAttributes;
using Microsoft.Xrm.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToDynamicsCrm
{
    [ConnectionModuleAttribute(
        DisplayName="Dynamics CRM", 
        Name="ConnectToDynamicsCrm", 
        ContainsSubConfiguration=false, 
        ModuleType=ModuleType.Connection,
        ConnectionType = typeof(CrmConnection),
        ConfigurationType=typeof(ConnectToDynamicsCrmConfiguration))]
    public class ConnectToDynamicsCrm : IModule, IConnection
    {
        public ConnectToDynamicsCrmConfiguration Configuration { get; set; }

        public object GetConnection()
        {
            switch(Configuration.ConnectionVersion)
            {
                case "2011":
                case "2013":
                    return (object)Crm2013Wrapper.Crm2013Wrapper.GetConnection(Configuration.ConnectionString);

                default:
                    return (object)Dynamics365Wrapper.GetConnection(Configuration.ConnectionString);
            }
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToDynamicsCrmConfiguration)configurationBase);
            return configurationWindow;
        }


        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToDynamicsCrmConfiguration;
        }

        
    }
}
