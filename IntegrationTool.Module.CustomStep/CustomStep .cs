using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.Module.CustomStep
{
    [StepModuleAttribute(
    // Defined once and not changed anymore!
    Name = "CustomStep",

    // Is shown in the editor
    DisplayName = "Custom Step",

    // Step, Source, Transformation, etc...
    ModuleType = ModuleType.Step,

    // Depending on the RequiresConnection-field, 
    // connections are shown in the configuration-window or not
    RequiresConnection = false,

    // Type which implements the configuration
    ConfigurationType = typeof(CustomStepConfiguration))]
    public class CustomStep : IModule, IStep
    {
        public CustomStepConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as CustomStepConfiguration;
        }

        public UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((CustomStepConfiguration)configurationBase);
            return configurationWindow;
        }

        public void Execute(IConnection connection, IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Yeah, we run our code now!"));

            reportProgress(new SimpleProgressReport("MyConfigValue is " + this.Configuration.MyConfigValue));
            // Your execution code goes here
        }
    }
}
