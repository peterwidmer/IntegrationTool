using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.MSSQLExecute
{
    public enum ExecutionType 
    {
        ExecuteNonQuery
    }

    public class MSSQLExecuteConfiguration : StepConfiguration
    {
        public ExecutionType ExecutionType { get; set; }
        public string SqlValue { get; set; }
    }
}
