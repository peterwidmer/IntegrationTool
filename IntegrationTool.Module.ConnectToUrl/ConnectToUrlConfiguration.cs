using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToUrl
{
    public class ConnectToUrlConfiguration : ConnectionConfigurationBase
    {
        public string Url { get; set; }
        public bool UseProxySettings { get; set; }
    }
}
