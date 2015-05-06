using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.TestErrorDelay
{
    public class TestErrorDelayConfiguration : StepConfiguration
    {
        public int DelayInMilliseconds { get; set; }
    }
}
