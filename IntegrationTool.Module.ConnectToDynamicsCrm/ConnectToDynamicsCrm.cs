using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Module.ModuleAttributes;
using Microsoft.Xrm.Sdk;
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
        ConnectionType = typeof(IOrganizationService),
        ConfigurationType=typeof(ConnectToDynamicsCrmConfiguration))]
    public class ConnectToDynamicsCrm : IModule, IConnection
    {
        public ConnectToDynamicsCrmConfiguration Configuration { get; set; }

        public object GetConnection()
        {
            if(Configuration.ConnectionVersion == null)
            {
                Configuration.ConnectionVersion = "CRM 2011";
            }

            switch(Configuration.ConnectionVersion)
            {
                case "CRM 2011":
                case "CRM 2013":
                case "CRM 2015":
                case "CRM 2016":
                    return (object)Crm2013Wrapper.Crm2013Wrapper.GetConnection(Configuration.ConnectionString);

                case "Dynamics 365":
                    return (object)Dynamics365Wrapper.GetConnection(Configuration.ConnectionString);

                default:
                    throw new ArgumentOutOfRangeException("CRM Connectionversion did not match!");
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
