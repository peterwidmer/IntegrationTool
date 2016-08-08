using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Diagram
{
    public class ConnectionBase
    {
        public Guid SourceID { get; set; }
        public ConnectorType ConnectionType { get; set; }
        public Guid SinkID { get; set; }

        public ConnectionBase() { }

        public ConnectionBase(Guid sourceId, Guid sinkId)
        {
            this.SourceID = sourceId;
            this.SinkID = sinkId;
        }
    }
}
