using IntegrationTool.SDK;
using IntegrationTool.SDK.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToDynamicsCrm
{
    public class ConnectToDynamicsCrmConfiguration : ConnectionConfigurationBase, IConfigurationValidation
    {
        /// <summary>
        /// Depicts the crm-version (i.e. 2011, 2013)
        /// </summary>
        public string ConnectionVersion { get; set; }

        /// <summary>
        /// Connectonstring to the crm-instance
        /// </summary>
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
