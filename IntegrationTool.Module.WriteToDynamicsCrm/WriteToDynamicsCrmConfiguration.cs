using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm
{
    public class WriteToDynamicsCrmConfiguration : TargetConfiguration
    {
        public string EntityName { get; set; }
        public ImportMode ImportMode { get; set; }
        public MultipleFoundMode MultipleFoundMode { get; set; }
        public LookupResolve LookupResolve { get; set; }
        public ImportMode SetStateMode { get; set; }
        public ImportMode SetOwnerMode { get; set; }
        public List<DataMapping> Mapping { get; set; } = new List<DataMapping>();
        public List<PicklistMapping> PicklistMapping { get; set; } = new List<PicklistMapping>();
        public ObservableCollection<string> PrimaryKeyAttributes { get; set; } = new ObservableCollection<string>();
        public List<RelationMapping> RelationMapping { get; set; } = new List<RelationMapping>();
        public int BatchSizeResolving { get; set; } = 100;

        public string [] GetAllMappedAttributes()
        {
            List<string> mappedAttributes = new List<string>();
            mappedAttributes.AddRange(Mapping.Select(t => t.Target));
            mappedAttributes.AddRange(RelationMapping.Select(t => t.LogicalName));

            return mappedAttributes.ToArray();
        }
    }
}
