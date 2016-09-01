using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class FlowGraph
    {
        public BlockingCollection<DesignerItemBase> DesignerItems { get; set; }
        public BlockingCollection<ConnectionBase> DesignerConnections { get; set; }

        public FlowGraph(BlockingCollection<DesignerItemBase> designerItems, BlockingCollection<ConnectionBase> designerConnections)
        {
            this.DesignerItems = designerItems;
            this.DesignerConnections = designerConnections;
        }

        public List<DesignerItemBase> GetStartNodesByNodeId(Guid designerItemId)
        {
            var startNodes = new List<DesignerItemBase>();
            GetStartNodes(new List<ConnectionBase>() { new ConnectionBase(designerItemId, Guid.Empty) }, startNodes);
            return startNodes;
        }

        public List<DesignerItemBase> GetPredecessorNodes(DesignerItemBase designerItem)
        {
            var incomingConnectionSourceIds = DesignerConnections.Where(t => t.SinkID == designerItem.ID).Select(t=> t.SourceID);
            var predecessorNodes = DesignerItems.Where(t => incomingConnectionSourceIds.Contains(t.ID));
            return predecessorNodes.ToList();
        }

        private void GetStartNodes(IEnumerable<ConnectionBase> connections, List<DesignerItemBase> startNodes)
        {
            foreach(var connection in connections)
            {
                var currentDesignerItem = GetCurrentDesignerItem(connection.SourceID);

                var incomingConnections = DesignerConnections.Where(t => t.SinkID == currentDesignerItem.ID);
                if (!incomingConnections.Any())
                {
                    if(currentDesignerItem.ModuleDescription.Attributes.ModuleType != SDK.ModuleType.Source)
                    {
                        throw new Exception("The module " + currentDesignerItem.ItemLabel + " is not a valid source!");
                    }
                    startNodes.Add(currentDesignerItem);
                }
                else
                {
                    GetStartNodes(incomingConnections, startNodes);
                }
            }
        }

        public DesignerItemBase GetCurrentDesignerItem(Guid designerItemId)
        {
            var currentDesignerItem = DesignerItems.FirstOrDefault(designerItem => designerItem.ID == designerItemId);
            if (currentDesignerItem == null)
            {
                throw new ArgumentOutOfRangeException("A DesignerItem with id " + designerItemId + " does not exist!");
            }

            return currentDesignerItem;
        }
    }
}
