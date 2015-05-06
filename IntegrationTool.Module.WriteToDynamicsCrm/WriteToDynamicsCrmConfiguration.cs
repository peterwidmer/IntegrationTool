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
        public ImportMode SetStateMode { get; set; }
        public ImportMode SetOwnerMode { get; set; }
        public List<DataMapping> Mapping { get; set; }
        public List<PicklistMapping> PicklistMapping { get; set; }
        public ObservableCollection<string> PrimaryKeyAttributes { get; set; }
        public List<RelationMapping> RelationMapping { get; set; }

        public WriteToDynamicsCrmConfiguration()
        {
            Mapping = new List<DataMapping>();
            PicklistMapping = new List<SDK.PicklistMapping>();
            PrimaryKeyAttributes = new ObservableCollection<string>();
            RelationMapping = new List<RelationMapping>();
        }
    }
}
