using IntegrationTool.Module.WriteToDynamicsCrm.Logging;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Logging;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm
{
    [TargetModuleAttribute(Name = "WriteToDynamicsCrm",
                           DisplayName = "CRM",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(CrmConnection),
                           ConfigurationType= typeof(WriteToDynamicsCrmConfiguration))]
    public partial class WriteToDynamicsCrm : IModule, IDataTarget, ILogRendering
    {
        public WriteToDynamicsCrmConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configuration, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToDynamicsCrmConfiguration)configuration, dataObject);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToDynamicsCrmConfiguration;
        }

        public System.Windows.Controls.UserControl RenderLogWindow(IntegrationTool.SDK.Database.IDatabaseInterface databaseInterface)
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
