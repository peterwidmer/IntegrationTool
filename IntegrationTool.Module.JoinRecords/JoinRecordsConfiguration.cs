using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.JoinRecords
{
    public enum JoinRecordsJoinType
    {
        LeftJoin, InnerJoin
    }

    public class JoinRecordsConfiguration : TransformationConfiguration
    {
        public JoinRecordsJoinType JoinType { get; set; }

        public List<DataMapping> JoinMapping { get; set; }

        public ObservableCollection<OutputColumn> OutputColumns { get; set; }

        public JoinRecordsConfiguration()
        {
            JoinMapping = new List<DataMapping>();
            OutputColumns = new ObservableCollection<OutputColumn>();
        }
    }
}
