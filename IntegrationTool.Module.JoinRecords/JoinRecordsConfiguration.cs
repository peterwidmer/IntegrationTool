using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.JoinRecords
{
    public enum JoinRecordsJoinType
    {
        InnerJoin
    }

    public class JoinRecordsConfiguration : TransformationConfiguration
    {
        public JoinRecordsJoinType JoinType { get; set; }

        public List<DataMapping> JoinMapping { get; set; }

        public List<OutputColumn> OutputColumns { get; set; }

        public JoinRecordsConfiguration()
        {
            JoinMapping = new List<DataMapping>();
            OutputColumns = new List<OutputColumn>();
            OutputColumns.Add(new OutputColumn() { Column = new ColumnMetadata("test"), DataStream = DataStreamSource.Left });
        }
    }
}
