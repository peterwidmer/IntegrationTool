using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.DeleteInDynamicsCrm.Logging.RecordLogList
{
    public class RecordLog
    {
        public int RecordLogId { get; set; }
        public string CombinedBusinessKey { get; set; }
        public string EntityIds { get; set; }
        public string DeletionFault { get; set; }
    }
}
