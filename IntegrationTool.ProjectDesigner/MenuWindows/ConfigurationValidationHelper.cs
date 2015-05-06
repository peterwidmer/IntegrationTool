using IntegrationTool.SDK;
using IntegrationTool.SDK.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrationTool.ProjectDesigner.MenuWindows
{
    public class ConfigurationValidationHelper
    {
        public static bool ValidateCurrentConfiguration(ConfigurationBase configuration)
        {
            IConfigurationValidation configurationValidation = configuration as IConfigurationValidation;
            if (configurationValidation != null)
            {
                var validationResult = configurationValidation.ValidateConfiguration();
                if (validationResult.ValidationSuccessful == false)
                {
                    string message = "";
                    foreach (string error in validationResult.Errors)
                    {
                        message += error + "\n";
                    }
                    MessageBox.Show(message, "Configuration Error", MessageBoxButton.OK);
                }

                return validationResult.ValidationSuccessful;
            }

            return true; // In case of no validation, return validation as valid
        }
    }
}
