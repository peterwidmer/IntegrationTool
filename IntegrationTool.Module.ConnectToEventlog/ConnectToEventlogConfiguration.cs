using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToEventlog
{
    public class ConnectToEventlogConfiguration : ConnectionConfigurationBase
    {
        public string Source { get; set; }
        public string LogName { get; set; }
    }
}
