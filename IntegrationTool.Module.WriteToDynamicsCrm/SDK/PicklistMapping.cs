using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.SDK
{
    public class PicklistMapping
    {
        public string LogicalName { get; set; }
        public PicklistMappingType MappingType { get; set; }
        public MappingNotFoundType MappingNotFound { get; set; }
        public List<DataMapping> Mapping { get; set; }
        public string DefaultValue { get; set; }

        public PicklistMapping()
        {
            Mapping = new List<DataMapping>();
        }
    }
}
