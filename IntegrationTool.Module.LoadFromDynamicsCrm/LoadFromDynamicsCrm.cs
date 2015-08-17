using IntegrationTool.SDK;
using Microsoft.Xrm.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromDynamicsCrm
{
    [SourceModuleAttribute(Name = "LoadFromDynamicsCrm",
                           DisplayName = "CRM",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(CrmConnection),
                           ConfigurationType = typeof(LoadFromDynamicsCrmConfiguration))]
    public class LoadFromDynamicsCrm
    {
    }
}
