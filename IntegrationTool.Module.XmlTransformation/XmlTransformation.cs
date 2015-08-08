using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.Module.XmlTransformation
{
    [TransformationModuleAttribute(
        Name = "XmlTransformation",
        DisplayName = "XML Transformation",
        ModuleType = ModuleType.Transformation,
        GroupName = ModuleGroup.Transformation,
        ConfigurationType = typeof(XmlTransformationConfiguration),
        RequiresConnection = false)]
    public partial class XmlTransformation : IModule, IDataTransformation
    {
        public XmlTransformationConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as XmlTransformationConfiguration;
        }

        public UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            return new ConfigurationWindow(configurationBase as XmlTransformationConfiguration, dataObject);
        }        
    }
}
