using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ExecuteCmd
{
    [StepModuleAttribute(Name = "ExecuteCmd",
                           DisplayName = "Execute CMD",
                           ModuleType = ModuleType.Step,
                           GroupName = ModuleGroup.Task,
                           ContainsSubConfiguration = false,
                           RequiresConnection = false,
                           ConfigurationType = typeof(ExecuteCmdConfiguration))]
    public class ExecuteCmd : IModule, IStep
    {
        public ExecuteCmdConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ExecuteCmdConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            throw new NotImplementedException();
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            throw new NotImplementedException();
        }
    }
}
