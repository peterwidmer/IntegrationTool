using IntegrationTool.SDK;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToSharepoint
{
    [ConnectionModuleAttribute(
        DisplayName = "Sharepoint",
        Name = "ConnectToSharepoint",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(string),
        ConfigurationType = typeof(ConnectToSharepointConfiguration))]
    public class ConnectToSharepoint
    {
    }
}
