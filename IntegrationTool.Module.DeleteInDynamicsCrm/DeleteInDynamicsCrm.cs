using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
        [TargetModuleAttribute(Name = "DeleteInDynamicsCrm",
                           DisplayName = "Delete CRM records",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(CrmConnection),
                           ConfigurationType = typeof(DeleteInDynamicsCrmConfiguration))]
    public partial class DeleteInDynamicsCrm : IModule, IDataTarget
    {

        public DeleteInDynamicsCrmConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configuration, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((DeleteInDynamicsCrmConfiguration)configuration, dataObject);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as DeleteInDynamicsCrmConfiguration;
        }        
    }
}
