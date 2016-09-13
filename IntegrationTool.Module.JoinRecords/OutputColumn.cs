using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.JoinRecords
{
    public enum DataStreamSource
    {
        [Description("Left Source")]
        Left=1,

        [Description("Right Source")]
        Right=2
    }

    public class OutputColumn
    {
        public DataStreamSource DataStream { get; set; }
        public ColumnMetadata Column { get; set; }
    }
}
