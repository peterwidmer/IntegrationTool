using IntegrationTool.DBAccess;
using IntegrationTool.SDK;
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

        public BlockingCollection<DesignerItemBase> designerItems;
        public BlockingCollection<ConnectionBase> designerConnections;
        private DesignerItemBase parentDesignerItem;
        private ItemLog parentItemLog;

        public SubFlowExecution(DesignerItemBase parentDesignerItem, ItemLog parentItemLog, ObjectResolver objectResolver, BlockingCollection<DesignerItemBase> designerItems, BlockingCollection<ConnectionBase> designerConnections)
        {
            this.parentDesignerItem = parentDesignerItem;
            this.parentItemLog = parentItemLog;
            this.objectResolver = objectResolver; 
            this.designerItems = designerItems;
            this.designerConnections = designerConnections;     
        }

        public void Execute(RunLog runLog)
        {
            IDatastore dataObject = GetDataObjectForDesignerItem(Guid.Empty, runLog);
            WriteDataToTarget(dataObject, runLog);
        }

        private void WriteDataToTarget(IDatastore dataObject, RunLog runLog)
        {
            DesignerItemBase targetItem = this.designerItems.Where(t => t.ModuleDescription.Attributes.ModuleType == ModuleType.Target).FirstOrDefault();
            if (targetItem == null)
            {
                throw new Exception("Could not find any targets to write data to!");
            }

            IModule targetModule = objectResolver.GetModule(targetItem.ID, targetItem.ModuleDescription.ModuleType);
            IConnection connectionObject = objectResolver.GetConnection(targetItem.ID);
            IDatabaseInterface databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, targetItem.ID, targetItem.ItemLabel);

            ItemLog itemLog = GetItemLog(targetItem.ID, targetItem.ItemLabel, targetItem.ModuleDescription.ModuleType.Name, runLog.RunLogPath + "\\" + databaseInterface.GetDatabaseName());

            ((IDataTarget)targetModule).WriteData(connectionObject, databaseInterface, dataObject, ReportProgressMethod);
            
            itemLog.EndTime = DateTime.Now;
            parentItemLog.SubFlowLogs.Add(itemLog);
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
            if (this.parentDesignerItem != null)
            {
                this.parentDesignerItem.BackgroundWorker.ReportProgress(100, new ProgressReport() { DesignerItem = this.parentDesignerItem, State = ItemEvent.ProgressReport, Message = progress.Message });
            }
        }

        public IDatastore GetDataObjectForDesignerItem(Guid loadUntildesignerItemId, RunLog runLog)
        {
            // Get source
            DesignerItemBase sourceItem = this.designerItems.FirstOrDefault(t => t.ModuleDescription.Attributes.ModuleType == ModuleType.Source);
            if (sourceItem == null)
            {
                throw new Exception("Could not find any sources to load data!");
            }

            IModule sourceObject = objectResolver.GetModule(sourceItem.ID, sourceItem.ModuleDescription.ModuleType);
            IConnection connectionObject = objectResolver.GetConnection(sourceItem.ID);
            if(connectionObject == null)
            {
                if(runLog != null)
                {
                    string label = String.IsNullOrEmpty(sourceItem.ItemLabel) ? "-- No Label --" : sourceItem.ItemLabel;
                    throw new Exception("No connection was selected for the source '" + label + "'.");
                }
                return null;  // TODO Decide if dummy datastore should be returned!
            }
            IDatabaseInterface databaseInterface = null;            

            ItemLog itemLog = GetItemLog(sourceItem.ID, sourceItem.ItemLabel, sourceItem.ModuleDescription.ModuleType.Name, null);

            if (runLog != null)
            {
                databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, sourceItem.ID, sourceItem.ItemLabel);
                itemLog.DatabasePath = runLog.RunLogPath + "\\" + databaseInterface.GetDatabaseName();
                parentItemLog.SubFlowLogs.Add(itemLog);
            }

            IDatastore dataObject = new DataObject();

            List<AttributeImplementation> dataConditionAttributes = AssemblyHelper.LoadAllClassesImplementingSpecificAttribute<DataConditionAttribute>(System.Reflection.Assembly.GetAssembly(typeof(DataConditionAttribute)));
            dataObject.InitializeDatastore(dataConditionAttributes);

            ((IDataSource)sourceObject).LoadData(connectionObject, dataObject, ReportProgressMethod);

            itemLog.EndTime = DateTime.Now;

            if(sourceItem.ID == loadUntildesignerItemId)
            {
                return dataObject;
            }

            //Transform data
            Guid lastDesignerItemId = sourceItem.ID;
            while(true)
            {
                if(lastDesignerItemId == Guid.Empty || 
                    designerConnections.Where(t=> t.SourceID == lastDesignerItemId).Count() == 0 ||
                    lastDesignerItemId == loadUntildesignerItemId)
                {
                    break;
                }

                Guid sinkDesignerItemId = designerConnections.Where(t => t.SourceID == lastDesignerItemId).First().SinkID;

                DesignerItemBase transformationDesignerItem = this.designerItems.Where(t => t.ID == sinkDesignerItemId).FirstOrDefault();
                if (transformationDesignerItem == null)
                {
                    throw new Exception("Could not find designerIten to transform data!");
                }
                if(transformationDesignerItem.ModuleDescription.Attributes.ModuleType != ModuleType.Transformation)
                {
                    break; // Obviously the targetitem, so we need to break here
                }

                var transformationItemConfiguration = objectResolver.LoadItemConfiguration(transformationDesignerItem.ID) as TransformationConfiguration;

                IConnection transformationConnectionObject = null;
                if (transformationItemConfiguration.SelectedConnectionConfigurationId.Equals(Guid.Empty) == false)
                {
                    transformationConnectionObject = objectResolver.GetConnection(transformationItemConfiguration.SelectedConnectionConfigurationId);
                }
                dataObject.ApplyFilter(transformationItemConfiguration.DataFilter);

                IModule transformationObject = objectResolver.GetModule(transformationDesignerItem.ID, transformationDesignerItem.ModuleDescription.ModuleType);
                ((IDataTransformation)transformationObject).TransformData(transformationConnectionObject, null, dataObject, ReportProgressMethod);
                dataObject.ClearFilter();

                lastDesignerItemId = transformationDesignerItem.ID;
            }

            return dataObject;
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
