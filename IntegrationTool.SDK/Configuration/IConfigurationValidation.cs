using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Configuration
{
    public interface IConfigurationValidation
    {
        ConfigurationValidationResult ValidateConfiguration();
    }
}
