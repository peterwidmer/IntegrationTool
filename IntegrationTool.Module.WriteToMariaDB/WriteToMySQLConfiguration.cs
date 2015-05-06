using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToMySQL.SDK.Enums;
using IntegrationTool.SDK;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToMySql
{
    public class WriteToMySQLConfiguration : CommonDbTargetConfiguration
    {
        public MySqlImportMode ImportMode { get; set; }
        public MySqlMultipleFoundMode MultipleFoundMode { get; set; }
        public ObservableCollection<string> PrimaryKeyAttributes { get; set; }
        public List<DataMapping> Mapping { get; set; }

        public WriteToMySQLConfiguration()
        {
            Mapping = new List<DataMapping>();
            PrimaryKeyAttributes = new ObservableCollection<string>();
        }
    }
}
