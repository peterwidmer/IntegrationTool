using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class SequentialExecutionPlanner
    {
        public List<DesignerItemBase> ExecutionPlan { get; set; }

        private FlowGraph flowGraph;

        public SequentialExecutionPlanner(FlowGraph flowGraph)
        {
            this.flowGraph = flowGraph;
            ExecutionPlan = new List<DesignerItemBase>();
        }

        public void CreateExecutionPlan(List<DesignerItemBase> targetItems)
        {
            foreach(var item in targetItems)
            {
                TraverseUpwards(item);
            }
            ExecutionPlan.Reverse();
        }

        public void TraverseUpwards(DesignerItemBase item)
        {
            if (!ExecutionPlan.Contains(item))
            {
                ExecutionPlan.Add(item);
            }

            var predessorItems = flowGraph.GetPredecessorNodes(item);
            foreach(var predessorItem in predessorItems)
            {
                TraverseUpwards(predessorItem);
            }
        }
    }
}
