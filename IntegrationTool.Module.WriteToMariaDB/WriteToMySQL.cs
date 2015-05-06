using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToMySql
{
    [TargetModuleAttribute(Name = "WriteToMySQL",
                           DisplayName = "MySQL",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(OdbcConnection),
                           ConfigurationType = typeof(WriteToMySQLConfiguration))]
    public partial class WriteToMySQL : IModule, IDataTarget
    {
        public WriteToMySQLConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToMySQLConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToMySQLConfiguration)configurationBase, dataObject);
            return configurationWindow;
        }

        
    }
}
