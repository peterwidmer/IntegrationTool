using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.JoinRecords
{
    [TransformationModuleAttribute(
        Name = "JoinRecords",
        DisplayName = "Join Records",
        ModuleType = ModuleType.Transformation,
        GroupName = ModuleGroup.Transformation,
        ConfigurationType = typeof(JoinRecordsConfiguration),
        RequiresConnection = false)]
    public partial class JoinRecords : IModule, IDataMerge
    {
        public JoinRecordsConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as JoinRecordsConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            return new ConfigurationWindow(configurationBase as JoinRecordsConfiguration, dataObject);
        }
    }
}
