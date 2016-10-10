using IntegrationTool.DiagramDesigner;
using IntegrationTool.ApplicationCore;
using IntegrationTool.ApplicationCore.Serialization;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using IntegrationTool.SDK.Exceptions;

namespace IntegrationTool.ProjectDesigner.MenuWindows
{
    public class ConfigurationWindowSettings
    {
        public string title {get; set;} 
        public Image titleImage {get; set; } 
        public IEnumerable<ConnectionConfigurationBase> connections {get; set; }
        public UserControl configurationControl {get; set; }
        public ModuleDescription moduleDescription {get; set; }
        public string originalConfiguration { get; set; }
        public StepConfigurationBase configuration {get; set; }
        public IDatastore datastore { get; set; }

        public static ConfigurationWindowSettings Get(DesignerItem designerItem, StepConfigurationBase configuration, ModuleLoader moduleLoader, List<IDatastore> dataStores, IEnumerable<ConnectionConfigurationBase> connections)
        {
            var module = Activator.CreateInstance(designerItem.ModuleDescription.ModuleType) as IModule;

            ConfigurationWindowSettings configurationWindowSettings = new ConfigurationWindowSettings()
            {
                title = designerItem.ItemLabel,
                titleImage = IntegrationTool.SDK.Diagram.IconLoader.GetFromAssembly(designerItem.ModuleDescription.ModuleType.Assembly, "Icon.xml"),
                connections = connections.Where(t => t.ModuleDescription.Attributes.ConnectionType == designerItem.ModuleDescription.Attributes.ConnectionType),
                moduleDescription = designerItem.ModuleDescription,
                configuration = configuration,
                originalConfiguration = ConfigurationSerializer.SerializeObject(configuration, moduleLoader.GetModuleTypeList()),
            };

            if(!dataStores.Any())
            {
                throw new InfoException("No incoming data available. Please connect a datasource first!");
            }

            if(module is IDataMerge)
            {
                configurationWindowSettings.configurationControl = ((IDataMerge)module).RenderConfigurationWindow(configuration, dataStores[0], dataStores[1]);
            }
            else
            {
                configurationWindowSettings.configurationControl = ((IModule)module).RenderConfigurationWindow(configuration, dataStores.First());
                configurationWindowSettings.datastore = dataStores.First();
            }

            return configurationWindowSettings;
        }
    }
}
