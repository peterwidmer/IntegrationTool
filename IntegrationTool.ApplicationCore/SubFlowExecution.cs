using IntegrationTool.DBAccess;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Data.DataConditionClasses;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class SubFlowExecution
    {
        private ObjectResolver objectResolver;
        private ItemWorker parentItemWorker;
        private ItemLog parentItemLog;
        public FlowGraph FlowGraph { get; set; }
        public Dictionary<ConnectionBase, IDatastore> DataStreams;
        private ModuleExecution moduleExecution;

        public SubFlowExecution(ItemWorker parentItemWorker, ItemLog parentItemLog, ObjectResolver objectResolver, FlowGraph flowGraph)
        {
            this.parentItemWorker = parentItemWorker;
            this.parentItemLog = parentItemLog;
            this.objectResolver = objectResolver;
            this.FlowGraph = flowGraph;
            this.DataStreams = new Dictionary<ConnectionBase, IDatastore>();            
        }

        public void Execute(RunLog runLog)
        {
            moduleExecution = new ModuleExecution(objectResolver, runLog, parentItemLog);

            var targetItems = this.FlowGraph.DesignerItems.Where(t => t.ModuleDescription.Attributes.ModuleType == ModuleType.Target).ToList();
            var executionPlan = CreateExecutionPlan(targetItems);
            ExecuteExecutionPlan(executionPlan, runLog, false);
        }

        public void ExecuteExecutionPlan(List<DesignerItemBase> executionPlan, RunLog runLog, bool mappingPreview)
        {
            foreach (var item in executionPlan)
            {
                ExecuteItem(item, runLog, mappingPreview);
            }  
        }

        public void ExecuteItem(DesignerItemBase item, RunLog runLog, bool mappingPreview)
        {
            var incomingConnections = FlowGraph.GetIncomingConnections(item);
            var outgoingConnections = FlowGraph.GetOutgoingConnections(item);

            List<IDatastore> dataStores = new List<IDatastore>();
            if (item.ModuleDescription.Attributes.ModuleType == ModuleType.Source)
            {
                var dataStore = InitializeDatastore();
                dataStores.Add(dataStore);
            }
            else
            {
                foreach(var incomingConnection in incomingConnections.OrderBy(t=> t.SinkConnectorName))
                {
                    dataStores.Add(DataStreams[incomingConnection]);
                    DataStreams.Remove(incomingConnection);
                }
            }

            var returnedDataStore = moduleExecution.ExecuteDesignerItem(item, dataStores, ReportProgressMethod, mappingPreview);
            
            for(int i = 0; i < outgoingConnections.Count; i++)
            {
                if (i == 0)
                {
                    DataStreams.Add(outgoingConnections[i], returnedDataStore);
                }
                else
                {
                    var targetDatastore = InitializeDatastore();
                    DatastoreHelper.CopyDatastore(returnedDataStore, targetDatastore);
                    DataStreams.Add(outgoingConnections[i], targetDatastore);
                }
            }
        }

        private IDatastore InitializeDatastore()
        {
            IDatastore dataStore = DataStoreFactory.GetDatastore();
            List<AttributeImplementation> dataConditionAttributes = AssemblyHelper.LoadAllClassesImplementingSpecificAttribute<DataConditionAttribute>(System.Reflection.Assembly.GetAssembly(typeof(DataConditionAttribute)));
            dataStore.InitializeDatastore(dataConditionAttributes);
            return dataStore;
        }

        private List<DesignerItemBase> CreateExecutionPlan(List<DesignerItemBase> targetItems)
        {
            var sequentialExecutionPlanner = new SequentialExecutionPlanner(this.FlowGraph);
            sequentialExecutionPlanner.CreateExecutionPlan(targetItems);
            return sequentialExecutionPlanner.ExecutionPlan;
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
            if (this.parentItemWorker != null)
            {
                this.parentItemWorker.BackgroundWorker.ReportProgress(100, new ProgressReport() { DesignerItem = this.parentItemWorker.DesignerItem, State = ItemEvent.ProgressReport, Message = progress.Message });
            }
        }

        public List<IDatastore> GetDataObjectForDesignerItem(Guid loadUntildesignerItemId, bool isDataPreview, bool ismappingPreview, RunLog runLog)
        {
            moduleExecution = new ModuleExecution(objectResolver, runLog, parentItemLog);

            var targetItems = this.FlowGraph.DesignerItems.Where(t => t.ID == loadUntildesignerItemId).ToList();
            var executionPlan = CreateExecutionPlan(targetItems);
            if(!isDataPreview)
            {
                executionPlan.Remove(targetItems.First());
            }
            ExecuteExecutionPlan(executionPlan, runLog, ismappingPreview);

            
            if (!isDataPreview)
            {
                var incomingConnections = FlowGraph.GetIncomingConnections(targetItems.First()).OrderBy(t => t.SinkConnectorName);
                return BuildResultStores(incomingConnections);
            }
            else
            {
                var outgoingConnections = FlowGraph.GetOutgoingConnections(targetItems.First()).OrderBy(t => t.SourceConnectorName);
                return BuildResultStores(outgoingConnections);
            }
        }

        private List<IDatastore> BuildResultStores(IEnumerable<ConnectionBase> connections)
        {
            List<IDatastore> resultStores = new List<IDatastore>();
            foreach (var connection in connections)
            {
                resultStores.Add(DataStreams[connection]);
            }
            return resultStores;
        }
    }
}
