using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Controls.Generic
{
    public class LogSummary
    {
        public int NumberOfRecordsLoaded { get; set; }
        public int NumberOfSuccessfulRecords { get; set; }
        public int NumberOfFailedRecords { get; set; }
    }
}
