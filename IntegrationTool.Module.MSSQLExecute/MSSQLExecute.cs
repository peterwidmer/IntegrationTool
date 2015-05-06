using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.MSSQLExecute
{
    [StepModuleAttribute(Name = "MSSQLExecute",
                           DisplayName = "MSSQL Execute",
                           ModuleType = ModuleType.Step,
                           GroupName = ModuleGroup.Task,
                           ContainsSubConfiguration = false,
                           RequiresConnection = true,
                           ConnectionType = typeof(SqlConnection),
                           ConfigurationType = typeof(MSSQLExecuteConfiguration))]
    public class MSSQLExecute : IModule, IStep
    {
        public MSSQLExecuteConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as MSSQLExecuteConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((MSSQLExecuteConfiguration)configurationBase);
            return configurationWindow;
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            using (SqlConnection sqlConnection = connection.GetConnection() as SqlConnection)
            {
                if (sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                using (SqlCommand sqlCommand = new SqlCommand(Configuration.SqlValue, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
