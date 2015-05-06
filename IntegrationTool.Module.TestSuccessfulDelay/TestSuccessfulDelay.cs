using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.TestSuccessfulDelay
{
    [StepModuleAttribute(Name = "SuccessfulDelay",
                           DisplayName = "Test Successful Delayed",
                           ModuleType = ModuleType.Step,
                           ContainsSubConfiguration = false,
                           RequiresConnection = false,
                           ConfigurationType = typeof(TestSuccessfulDelayConfiguration))]
    public class TestSuccessfulDelay : IModule, IStep
    {
        public TestSuccessfulDelayConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as TestSuccessfulDelayConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((TestSuccessfulDelayConfiguration)configurationBase, dataObject);
            return configurationWindow;
        }

        public void Execute(IConnection connection, IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            System.Threading.Thread.Sleep(this.Configuration.DelayInMilliseconds);
        }
    }
}
