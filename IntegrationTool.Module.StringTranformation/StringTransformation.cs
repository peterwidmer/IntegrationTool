using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.StringTranformation
{
    [TransformationModuleAttribute(
        Name = "StringTransformation",
        DisplayName = "String Transformation",
        ModuleType = ModuleType.Transformation,
        ConfigurationType = typeof(StringTransformationConfiguration),
        RequiresConnection=false)]
    public partial class StringTransformation : IModule, IDataTransformation
    {
        public StringTransformationConfiguration Configuration { get; set; }

        public StringTransformation()
        {
            Configuration = new StringTransformationConfiguration();
        }
        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as StringTransformationConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            List<StringTransformationAttribute> transformationAttributes = Helpers.LoadAllTransformationClasses();
            return new ConfigurationWindow(configurationBase as StringTransformationConfiguration, transformationAttributes, dataObject);
        }
        
    }
}
