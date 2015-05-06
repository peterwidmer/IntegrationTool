using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class RunLog
    {
        public Guid RunId { get; set; }
        public string RunLogPath { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid PackageId { get; set; }
        public string PackageDisplayName { get; set; }

        public List<ItemLog> ItemLogs { get; set; }

        public RunLog()
        {
            RunId = Guid.NewGuid();
            ItemLogs = new List<ItemLog>();
        }
    }
}
