using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class CommonDbTargetConfiguration : TargetConfiguration
    {
        public string TargetTable { get; set; }
        public ObservableCollection<string> PrimaryKeyFields { get; set; }
        public List<DataMapping> Mapping { get; set; }
    }
}
