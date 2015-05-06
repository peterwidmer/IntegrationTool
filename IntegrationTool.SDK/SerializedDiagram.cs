using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace IntegrationTool.SDK
{
    public class SerializedDiagram
    {
        public Guid ParentItemId { get; set; }
        public XElement Diagram { get; set; }
    }
}
