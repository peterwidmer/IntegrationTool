using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Logging
{
    public class RecordLog
    {
        public int RecordLogId { get; set; }
        public string CombinedBusinessKey { get; set; }
        public string ImportMode { get; set; }
        public string WriteError { get; set; }
    }
}
