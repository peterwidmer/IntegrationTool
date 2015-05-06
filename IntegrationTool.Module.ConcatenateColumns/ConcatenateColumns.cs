using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConcatenateColumns
{
    [TransformationModuleAttribute(
        Name = "ConcatenateColumns",
        DisplayName = "Concatenate Columns",
        ModuleType = ModuleType.Transformation,
        GroupName = ModuleGroup.Transformation,
        ConfigurationType = typeof(ConcatenateColumnsConfiguration),
        RequiresConnection = false)]
    public partial class ConcatenateColumns : IModule, IDataTransformation
    {
        public ConcatenateColumnsConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConcatenateColumnsConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            return new ConfigurationWindow(configurationBase as ConcatenateColumnsConfiguration, dataObject);
        }        
    }
}
