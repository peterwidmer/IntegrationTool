using IntegrationTool.Module.DeleteInDynamicsCrm.Logging;
using IntegrationTool.Module.DeleteInDynamicsCrm.Logging.RecordLogList;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Logging;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
        [TargetModuleAttribute(Name = "DeleteInDynamicsCrm",
                           DisplayName = "Delete CRM records",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(IOrganizationService),
                           ConfigurationType = typeof(DeleteInDynamicsCrmConfiguration))]
    public partial class DeleteInDynamicsCrm : IModule, IDataTarget, ILogRendering
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

        public System.Windows.Controls.UserControl RenderLogWindow(IDatabaseInterface databaseInterface)
        {
            LogWindow logWindow = new LogWindow();

            LogSummary logSummary = Logger.LoadLogSummary(databaseInterface);
            logWindow.LogSummaryControl.SetModel(logSummary);

            Logger logger = new Logger(databaseInterface);
            ObservableCollection<RecordLog> recordLogs = logger.ReadPagedRecords(1);
            logWindow.RecordLogListControl.SetModel(recordLogs);
            return logWindow;
        }
    }
}
