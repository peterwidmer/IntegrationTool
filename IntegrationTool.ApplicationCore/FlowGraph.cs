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
            this.DesignerConnections = DesignerConnections;
        }

        public List<DesignerItemBase> GetStartNodesByNodeId(Guid designerItemId)
        {
            var startNodes = new List<DesignerItemBase>();
            GetStartNodes(new List<ConnectionBase>() { new ConnectionBase(Guid.Empty, designerItemId) }, startNodes);
            return startNodes;
        }

        private void GetStartNodes(IEnumerable<ConnectionBase> connections, List<DesignerItemBase> startNodes)
        {
            foreach(var connection in connections)
            {
                var currentDesignerItem = GetCurrentDesignerItem(connection.SourceID);

                var incomingConnections = DesignerConnections.Where(t => t.SinkID == currentDesignerItem.ID);
                if (!incomingConnections.Any())
                {
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
