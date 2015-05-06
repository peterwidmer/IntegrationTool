using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Configuration
{
    public class ConfigurationValidationResult
    {
        public bool ValidationSuccessful
        {
            get { return (Errors.Count == 0); }
        }
        public List<string> Errors { get; set; }

        public ConfigurationValidationResult()
        {
            Errors = new List<string>();
        }
    }
}
