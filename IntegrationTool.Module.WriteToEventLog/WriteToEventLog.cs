using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToEventLog
{
    [StepModuleAttribute(Name = "WriteToEventLog",
                           DisplayName = "Write to eventlog",
                           ModuleType = ModuleType.Step,
                           ContainsSubConfiguration = false,
                           RequiresConnection = true,
                           ConnectionType=typeof(EventLog),
                           ConfigurationType = typeof(WriteToEventLogConfiguration))]
    public class WriteToEventLog : IModule, IStep
    {
        public WriteToEventLogConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToEventLogConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToEventLogConfiguration)configurationBase);
            return configurationWindow;
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            string[] eventLogConnection = connection.GetConnection() as string[];
            EventLog eventLog = new EventLog();
            eventLog.Source = eventLogConnection[0];
            eventLog.Log = eventLogConnection[1];
            EventLogEntryType level = (EventLogEntryType)Enum.Parse(typeof(EventLogEntryType), this.Configuration.Level.ToString());
            eventLog.WriteEntry(this.Configuration.Message, level);            
        }
    }
}
