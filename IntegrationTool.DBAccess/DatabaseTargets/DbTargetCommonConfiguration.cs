using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess.DatabaseTargets;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DbTargetCommonConfiguration : TargetConfiguration
    {
        public string TargetTable { get; set; }
        public ObservableCollection<string> PrimaryKeyFields { get; set; }
        public List<DataMapping> Mapping { get; set; }

        public DbTargetImportMode ImportMode { get; set; }
        public DbTargetMultipleFoundMode MultipleFoundMode { get; set; }

        public DbTargetCommonConfiguration()
        {
            Mapping = new List<DataMapping>();
            PrimaryKeyFields = new ObservableCollection<string>();
        }
    }
}
