using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToTextFile
{
    public class ConnectToTextFileConfiguration : ConnectionConfigurationBase
    {
        public string FilePath { get; set; }
    }
}
