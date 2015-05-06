using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.TestErrorDelay
{
    [StepModuleAttribute(Name = "ErrorDelay",
                           DisplayName = "Test Error Delayed",
                           ModuleType = ModuleType.Step,
                           ContainsSubConfiguration = false,
                           RequiresConnection = false,
                           ConfigurationType = typeof(TestErrorDelayConfiguration))]
    public class TestErrorDelay : IModule, IStep
    {
        public TestErrorDelayConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as TestErrorDelayConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((TestErrorDelayConfiguration)configurationBase, dataObject);
            return configurationWindow;
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            System.Threading.Thread.Sleep(this.Configuration.DelayInMilliseconds);
            throw new Exception("Error");
        }
    }
}
