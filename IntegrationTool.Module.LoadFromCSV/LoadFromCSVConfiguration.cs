using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromCSV
{
    public class LoadFromCSVConfiguration : SourceConfiguration
    {
        public string Delimiter { get; set; }
        public string Comment { get; set; }
        public string Quote { get; set; }
        public bool QuoteAllFields { get; set; }

        public LoadFromCSVConfiguration()
        {
            this.Delimiter = ",";
            this.Comment = "#";
            this.Quote = "\"";
            this.QuoteAllFields = false;
        }
    }
}
