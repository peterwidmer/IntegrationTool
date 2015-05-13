using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ODBCExecute
{
    public enum OdbcExecutionType
    {
        ExecuteNonQuery
    }

    public class ODBCExecuteConfiguration : StepConfiguration
    {
        public OdbcExecutionType ExecutionType { get; set; }
        public string SqlValue { get; set; }
    }
}
