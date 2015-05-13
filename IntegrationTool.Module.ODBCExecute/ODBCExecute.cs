using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ODBCExecute
{
    [StepModuleAttribute(Name = "ODBCExecute",
                           DisplayName = "ODBC Execute",
                           ModuleType = ModuleType.Step,
                           GroupName = ModuleGroup.Task,
                           ContainsSubConfiguration = false,
                           RequiresConnection = true,
                           ConnectionType = typeof(OdbcConnection),
                           ConfigurationType = typeof(ODBCExecuteConfiguration))]
    public class ODBCExecute : IModule, IStep
    {
        public ODBCExecuteConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ODBCExecuteConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ODBCExecuteConfiguration)configurationBase);
            return configurationWindow;
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            using (OdbcConnection sqlConnection = connection.GetConnection() as OdbcConnection)
            {
                if (sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                using (OdbcCommand odbcCommand = new OdbcCommand(Configuration.SqlValue, sqlConnection))
                {
                    odbcCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
