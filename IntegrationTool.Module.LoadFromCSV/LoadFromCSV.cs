using IntegrationTool.SDK;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromCSV
{
    [SourceModuleAttribute(Name = "LoadFromCSV",
                           DisplayName = "CSV",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(StringReader),
                           ConfigurationType = typeof(LoadFromCSVConfiguration))]
    public class LoadFromCSV : IModule, IDataSource
    {
        public LoadFromCSVConfiguration Configuration { get; set; }
        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromCSVConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            throw new NotImplementedException();
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress)
        {
            throw new NotImplementedException();
        }
    }
}
