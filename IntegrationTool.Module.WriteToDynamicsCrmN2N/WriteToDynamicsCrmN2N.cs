using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrmN2N
{
    [TargetModuleAttribute(Name = "WriteToDynamicsCrmN2N",
                           DisplayName = "CRM Many to many",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(IOrganizationService),
                           ConfigurationType = typeof(WriteToDynamicsCrmN2NConfiguration))]
    public partial class WriteToDynamicsCrmN2N : IModule, IDataTarget
    {
        public WriteToDynamicsCrmN2NConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configuration, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToDynamicsCrmN2NConfiguration)configuration, dataObject);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToDynamicsCrmN2NConfiguration;
        }
    }
}
