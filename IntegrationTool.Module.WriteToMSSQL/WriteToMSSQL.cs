using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToMSSQL
{
    [TargetModuleAttribute(Name = "WriteToMSSQL",
                           DisplayName = "MSSQL",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(SqlConnection),
                           ConfigurationType = typeof(WriteToMSSQLConfiguration))]
    public partial class WriteToMSSQL : IModule, IDataTarget
    {
        public WriteToMSSQLConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configuration, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToMSSQLConfiguration)configuration, dataObject);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToMSSQLConfiguration;
        }     
    }
}
