using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DataMappingControl
{
    public class DataMapping
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string ValueFormat { get; set; }

        public DataMapping() { }

        public DataMapping(string source, string target)
        {
            this.Source = source;
            this.Target = target;
        }
    }
}
