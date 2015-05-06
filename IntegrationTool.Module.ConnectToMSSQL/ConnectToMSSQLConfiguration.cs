using IntegrationTool.SDK;
using IntegrationTool.SDK.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToMSSQL
{
    public class ConnectToMSSQLConfiguration : ConnectionConfigurationBase, IConfigurationValidation
    {
        public string ConnectionString { get; set; }

        public ConfigurationValidationResult ValidateConfiguration()
        {
            ConfigurationValidationResult validationResult = new ConfigurationValidationResult();

            if (String.IsNullOrEmpty(ConnectionString))
            {
                validationResult.Errors.Add("Connectionstring must not be empty!");
            }

            return validationResult;
        }
    }
}
