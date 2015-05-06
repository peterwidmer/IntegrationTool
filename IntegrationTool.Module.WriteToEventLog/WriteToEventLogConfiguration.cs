using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToEventLog
{
    public enum EventLogLevel
    {
        Information, Warning, Error
    }

    public class WriteToEventLogConfiguration : StepConfiguration
    {
        public string Message { get; set; }

        public EventLogLevel Level { get; set; }
    }
}
