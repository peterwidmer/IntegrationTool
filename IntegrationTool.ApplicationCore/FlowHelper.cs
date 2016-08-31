using IntegrationTool.SDK;
using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class FlowHelper
    {
        public static bool AllowExecution_OnPreviousErrorTest(ItemWorker itemWorker, BlockingCollection<ItemWorker> itemWorkers, BlockingCollection<ConnectionBase> connectionList)
        {
            IEnumerable<ConnectionBase> incomingConnections = GetIncomingConnections(itemWorker.DesignerItem.ID, connectionList);
            foreach(ConnectionBase connection in incomingConnections.Where(t=> t.ConnectionType == ConnectorType.Default))
            {
                ItemWorker sourceItemWorker = itemWorkers.FirstOrDefault(t=> t.DesignerItem.ID == connection.SourceID);

                bool allowExecution = AllowExecution_OnPreviousErrorTest_Recursive(sourceItemWorker, itemWorkers, connectionList);
                if (allowExecution == false)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool AllowExecution_OnPreviousErrorTest_Recursive(ItemWorker itemWorker, BlockingCollection<ItemWorker> itemWorkers, BlockingCollection<ConnectionBase> connectionList)
        {
            if (itemWorker.DesignerItem.State == ItemState.Error && itemWorker.Configuration.OnError == StepExecutionErrorHandling.StopFollwingSteps)
            {
                return false;
            }

            IEnumerable<ConnectionBase> incomingConnections = GetIncomingConnections(itemWorker.DesignerItem.ID, connectionList);
            foreach (ConnectionBase connection in incomingConnections.Where(t=> t.ConnectionType == ConnectorType.Default))
            {
                ItemWorker sourceItemWorker = itemWorkers.Where(t => t.DesignerItem.ID == connection.SourceID).FirstOrDefault();
                bool allowExecution = AllowExecution_OnPreviousErrorTest_Recursive(sourceItemWorker, itemWorkers, connectionList);
                if(allowExecution == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AllowExecution_PreviousStepNotSuccessfulOnErrorlineTest(ItemWorker itemWorker, BlockingCollection<ItemWorker> itemWorkers, BlockingCollection<ConnectionBase> connectionList)
        {
            IEnumerable<ConnectionBase> incomingConnections = GetIncomingConnections(itemWorker.DesignerItem.ID, connectionList);
            
            // If any previous item is successful on successful line, continue is ok
            foreach (ConnectionBase connection in incomingConnections.Where(t=> t.ConnectionType == ConnectorType.Default))
            {
                ItemWorker sourceItemWorker = itemWorkers.Where(t => t.DesignerItem.ID == connection.SourceID).FirstOrDefault();
                if (sourceItemWorker.DesignerItem.State == ItemState.Stopped)
                {
                    return true;
                }
            }

            // If previous item is ok on errorline, continue is not allowed
            foreach (ConnectionBase connection in incomingConnections)
            {
                ItemWorker sourceItemWorker = itemWorkers.Where(t => t.DesignerItem.ID == connection.SourceID).FirstOrDefault();

                if (connection.ConnectionType == ConnectorType.Error && sourceItemWorker.DesignerItem.State == ItemState.Stopped)
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<ConnectionBase> GetIncomingConnections(Guid designerItemId, BlockingCollection<ConnectionBase> connectionList)
        {
            var qIncomingConnections = from connections in connectionList
                                       where connections.SinkID == designerItemId
                                       select connections;

            return qIncomingConnections;
        }
    }
}
