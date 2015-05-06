using IntegrationTool.SDK;
using IntegrationTool.SDK.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToExcel
{
    public class ConnectToExcelConfiguration : ConnectionConfigurationBase, IConfigurationValidation
    {
        public string FilePath { get; set; }

        public string SheetName { get; set; }

        public ConfigurationValidationResult ValidateConfiguration()
        {
            ConfigurationValidationResult validationResult = new ConfigurationValidationResult();

            if (String.IsNullOrEmpty(FilePath))
            {
                validationResult.Errors.Add("Filepath must not be empty!");
            }

            if (String.IsNullOrEmpty(SheetName))
            {
                validationResult.Errors.Add("SheetName must not be empty!");
            }

            return validationResult;
        }
    }
}
