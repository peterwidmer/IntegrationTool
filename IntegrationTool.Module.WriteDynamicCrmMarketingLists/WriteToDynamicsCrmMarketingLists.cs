using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists
{
    [TargetModuleAttribute(Name = "WriteToDynamicsCrmMarketingLists",
                           DisplayName = "CRM Marketing-List",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(CrmConnection),
                           ConfigurationType = typeof(WriteToDynamicsCrmMarketingListsConfiguration))]
    public class WriteToDynamicsCrmMarketingLists : IModule, IDataTarget
    {

        public WriteToDynamicsCrmMarketingListsConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configuration, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToDynamicsCrmMarketingListsConfiguration)configuration, dataObject);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToDynamicsCrmMarketingListsConfiguration;
        }

        public void WriteData(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, SDK.Database.IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            throw new NotImplementedException();
        }
    }
}
