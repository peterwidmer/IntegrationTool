using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public enum StepExecutionErrorHandling
    {
        ExecuteFollowingSteps, StopFollwingSteps
    }

    public enum StepExecutionStatus
    {
        Active, Inactive
    }

    public class StepConfigurationBase : ConfigurationBase
    {
        public Guid SelectedConnectionConfigurationId { get; set; }
        public StepExecutionErrorHandling OnError { get; set; }
        public StepExecutionStatus Status { get; set; }
    }
}
