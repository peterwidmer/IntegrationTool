using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.AddDefaultValue
{
    [TransformationModuleAttribute(
        Name = "AddDefaultValue",
        DisplayName = "Add default value",
        ModuleType = ModuleType.Transformation,
        ConfigurationType = typeof(AddDefaultValueConfiguration),
        RequiresConnection = false)]
    public partial class AddDefaultValue : IModule, IDataTransformation
    {
        public AddDefaultValueConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as AddDefaultValueConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            return new ConfigurationWindow(configurationBase as AddDefaultValueConfiguration, dataObject);
        }        
    }
}
