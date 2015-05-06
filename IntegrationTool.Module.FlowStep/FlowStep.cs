using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.FlowStep
{
    [StepModuleAttribute(Name = "FlowStep",
                           DisplayName = "Flow Step",
                           ModuleType = ModuleType.Step,
                           GroupName = ModuleGroup.Flow ,
                           ContainsSubConfiguration=true,
                           ConfigurationType=typeof(StepConfiguration))]
    public class FlowStep : IModule
    {
        private StepConfiguration stepConfiguration;

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            throw new NotImplementedException();
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.stepConfiguration = configurationBase as StepConfiguration;
        }
    }
}
