using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.SDK
{
    public class RelationMapping
    {
        public string LogicalName { get; set; }
        public string EntityName { get; set; }
        public RelationMappingType MappingType { get; set; }
        public List<DataMapping> Mapping { get; set; }

        public RelationMapping()
        {
            Mapping = new List<DataMapping>();
        }
    }
}
