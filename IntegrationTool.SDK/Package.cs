using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace IntegrationTool.SDK
{
    public class Package
    {
        [XmlIgnore]
        public Project ParentProject { get; set; }

        public Guid PackageId { get; set; }
        
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public SerializedDiagram Diagram { get; set; }
        public List<SerializedDiagram> SubDiagrams { get; set; }
        public List<ConfigurationBase> Configurations { get; set; }

        public Package()
        {
            Diagram = new SerializedDiagram();
            SubDiagrams = new List<SerializedDiagram>();
            Configurations = new List<ConfigurationBase>();
        }
    }
}
