using IntegrationTool.Module.LoadFromODBC.SDK.Enums;
using IntegrationTool.SDK.Configuration;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromODBC
{
    public class LoadFromODBCConfiguration : SourceConfiguration, IConfigurationValidation
    {
        public ODBCQueryType QueryType { get; set; }
        public string SqlValue { get; set; }

        public ConfigurationValidationResult ValidateConfiguration()
        {
            ConfigurationValidationResult validationResult = new ConfigurationValidationResult();

            switch (QueryType)
            {
                case SDK.Enums.ODBCQueryType.ODBCQuery:
                    if (String.IsNullOrEmpty(SqlValue))
                    {
                        validationResult.Errors.Add("SQL Query must not be empty!");
                    }
                    break;
            }

            return validationResult;
        }
    }
}
