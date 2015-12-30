using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrmN2N
{
    public class WriteToDynamicsCrmN2NConfiguration : TargetConfiguration
    {
        public string Entity1Name { get; set; }
        public string Entity2Name { get; set; }
        public string RelationshipName { get; set; }

        public List<DataMapping> Entity1Mapping { get; set; }
        public List<DataMapping> Entity2Mapping { get; set; }
    }
}
