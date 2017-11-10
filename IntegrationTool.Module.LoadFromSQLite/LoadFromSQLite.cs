using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromSQLite
{
    [SourceModuleAttribute(Name = "LoadFromSQLite",
                           DisplayName = "SQLite",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(SQLiteConnection),
                           ConfigurationType = typeof(LoadFromSQLiteConfiguration))]
    public class LoadFromSQLite : IModule, IDataSource
    {
        public LoadFromSQLiteConfiguration Configuration { get; set; }

        public LoadFromSQLite()
        {
            Configuration = new LoadFromSQLiteConfiguration();
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromSQLiteConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromSQLiteConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress)
        {

        }
    }
}
