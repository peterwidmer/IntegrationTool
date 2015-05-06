using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTool.SDK
{
    public class ConnectionConfigurationBase : ConfigurationBase
    {
        [XmlIgnore]
        private ModuleDescription moduleDescription;
        [XmlIgnore]
        public ModuleDescription ModuleDescription
        {
            get { 
                return this.moduleDescription; 
            }
            set
            {
                this.moduleDescription = value;
                ConnectionTypeName = value.ModuleType.AssemblyQualifiedName;
            }
        }

        public string ConnectionTypeName { get; set; }
        public ConnectionConfigurationBase()
        {
            if (ConfigurationId == Guid.Empty)
            {
                ConfigurationId = Guid.NewGuid();
            }
        }
    }
}
