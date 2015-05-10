using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ExecuteCmd
{
    public enum CmdExecutionType
    {
        cmd, bat
    }

    public class ExecuteCmdConfiguration : StepConfiguration
    {
        public CmdExecutionType ExecutionType { get; set; }
        public string CmdValue { get; set; }
    }
}
