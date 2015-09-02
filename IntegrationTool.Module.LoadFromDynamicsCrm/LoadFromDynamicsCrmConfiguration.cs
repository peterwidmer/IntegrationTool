using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromDynamicsCrm
{
    public enum DynamicsCrmQueryType
    {
        ExecuteFetchXml
    }

    public class LoadFromDynamicsCrmConfiguration : SourceConfiguration
    {
        public DynamicsCrmQueryType QueryType { get; set; }
        public string FetchXml { get; set; }
    }
}
