using IntegrationTool.Module.LoadFromSQLite.SDK.Enums;
using IntegrationTool.SDK.Configuration;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromSQLite
{
    public class LoadFromSQLiteConfiguration : SourceConfiguration, IConfigurationValidation
    {
        public SQLiteQueryType QueryType { get; set; }
        public string SqlValue { get; set; }

        public ConfigurationValidationResult ValidateConfiguration()
        {
            ConfigurationValidationResult validationResult = new ConfigurationValidationResult();

            switch (QueryType)
            {
                case SQLiteQueryType.SqlQuery:
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
