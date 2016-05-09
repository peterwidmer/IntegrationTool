using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromWebRequest
{
    [SourceModuleAttribute(Name = "LoadFromWebRequest",
                           DisplayName = "Web-Request",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(HttpWebRequest),
                           ConfigurationType = typeof(LoadFromWebRequestConfiguration))]
    public class LoadFromWebRequest : IModule, IDataSource
    {
        public LoadFromWebRequestConfiguration Configuration { get; set; }
        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromWebRequestConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromWebRequestConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress)
        {
        }
    }
}
