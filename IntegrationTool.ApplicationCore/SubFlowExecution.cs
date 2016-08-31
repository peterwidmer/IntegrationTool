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

        public SubFlowExecution(ItemWorker parentItemWorker, ItemLog parentItemLog, ObjectResolver objectResolver, FlowGraph flowGraph)
        {
            this.parentItemWorker = parentItemWorker;
            this.parentItemLog = parentItemLog;
            this.objectResolver = objectResolver;
            this.FlowGraph = flowGraph;
        }

        public void Execute(RunLog runLog)
        {
            DesignerItemBase targetItem = this.FlowGraph.DesignerItems.FirstOrDefault(t => t.ModuleDescription.Attributes.ModuleType == ModuleType.Target);
            if (targetItem == null)
            {
                throw new Exception("Could not find any targets to write data to!");
            }

            var dataStreams = GetDataObjectForDesignerItem(targetItem.ID, runLog);
            var dataStream = dataStreams.First();

            dataStream.WriteToTarget(targetItem, ReportProgressMethod);     
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
            if (this.parentItemWorker != null)
            {
                this.parentItemWorker.BackgroundWorker.ReportProgress(100, new ProgressReport() { DesignerItem = this.parentItemWorker.DesignerItem, State = ItemEvent.ProgressReport, Message = progress.Message });
            }
        }

        public List<DataStream> GetDataObjectForDesignerItem(Guid loadUntildesignerItemId, RunLog runLog)
        {
            // Get source
            var sources = this.FlowGraph.GetStartNodesByNodeId(loadUntildesignerItemId);
            if (sources.Count == 0)
            {
                throw new Exception("Could not find any sources to load data!");
            }

            List<DataStream> dataStreams = CreateDataStreams(sources, runLog);

            var dataStream = dataStreams.First();
            var sourceItem = sources[0];

            if(sourceItem.ID == loadUntildesignerItemId)
            {
                return dataStreams;
            }

            //Transform data
            Guid lastDesignerItemId = sourceItem.ID;
            while(true)
            {
                if(lastDesignerItemId == Guid.Empty ||
                    this.FlowGraph.DesignerConnections.Count(t => t.SourceID == lastDesignerItemId) == 0 ||
                    lastDesignerItemId == loadUntildesignerItemId)
                {
                    break;
                }

                Guid sinkDesignerItemId = this.FlowGraph.DesignerConnections.First(t => t.SourceID == lastDesignerItemId).SinkID;

                DesignerItemBase transformationDesignerItem = this.FlowGraph.DesignerItems.FirstOrDefault(t => t.ID == sinkDesignerItemId);
                if (transformationDesignerItem == null)
                {
                    throw new Exception("Could not find designerIten to transform data!");
                }
                if(transformationDesignerItem.ModuleDescription.Attributes.ModuleType != ModuleType.Transformation)
                {
                    break; // Obviously the targetitem, so we need to break here
                }

                dataStream.TransformData(transformationDesignerItem, ReportProgressMethod);

                dataStream.DataStore.ClearFilter();

                lastDesignerItemId = transformationDesignerItem.ID;
            }

            return dataStreams;
        }

        public List<DataStream> CreateDataStreams(List<DesignerItemBase> sourceNodes, RunLog runLog)
        {
            var dataStreams = new List<DataStream>();
            foreach (var source in sourceNodes)
            {
                var dataStream = new DataStream(new DataObject(), objectResolver, runLog, parentItemLog);
                dataStream.LoadDataFromSource(source, ReportProgressMethod);
                dataStreams.Add(dataStream);                
            }

            return dataStreams;
        }

        private ItemLog GetItemLog(Guid id, string itemLabel, string moduleDescriptionName, string databasePath)
        {
            ItemLog itemLog = new ItemLog()
            {
                DesignerItemId = id,
                DesignerItemDisplayName = itemLabel,
                ModuleDescriptionName = moduleDescriptionName,
                StartTime = DateTime.Now,
                DatabasePath = databasePath
            };

            return itemLog;
        }       
    }
}
