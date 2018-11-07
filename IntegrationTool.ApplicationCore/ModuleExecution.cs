using IntegrationTool.DBAccess;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data.DataConditionClasses;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ModuleExecution
    {
        private ObjectResolver objectResolver;
        private RunLog runLog;
        private ItemLog parentItemLog;

        public ModuleExecution(ObjectResolver objectResolver, RunLog runLog, ItemLog parentItemLog)
        {
            this.objectResolver = objectResolver;
            this.runLog = runLog;
            this.parentItemLog = parentItemLog;
        }

        public IDatastore ExecuteDesignerItem(DesignerItemBase designerItem, List<IDatastore> dataStores, ReportProgressMethod reportProgressMethod, bool mappingPreview)
        {
            switch(designerItem.ModuleDescription.Attributes.ModuleType)
            {
                case ModuleType.Source:
                    return LoadDataFromSource(designerItem, dataStores.First(), reportProgressMethod, mappingPreview);

                case ModuleType.Transformation:
                    return TransformData(designerItem, dataStores, reportProgressMethod);

                case ModuleType.Target:
                    return WriteToTarget(designerItem, dataStores.First(), reportProgressMethod);

                default:
                    throw new Exception("ModuleType " + designerItem.ModuleDescription.Attributes.ModuleType + " has no execution implemented");
            }
        }

        public IDatastore LoadDataFromSource(DesignerItemBase sourceItem, IDatastore dataStore, ReportProgressMethod reportProgressMethod, bool mappingPreview)
        {
            IModule sourceObject = objectResolver.GetModule(sourceItem.ID, sourceItem.ModuleDescription.ModuleType);
            IConnection connectionObject = objectResolver.GetConnection(sourceItem.ID);
            if (connectionObject == null)
            {
                if (runLog != null)
                {
                    string label = sourceItem.ItemLabel;
                    throw new Exception("No connection was selected for the source '" + label + "'.");
                }
                dataStore = null;
            }

            ItemLog itemLog = ItemLog.CreateNew(sourceItem.ID, sourceItem.ItemLabel, sourceItem.ModuleDescription.ModuleType.Name, null);

            if (runLog != null)
            {
                var databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, sourceItem.ID, sourceItem.ItemLabel);
                itemLog.DatabasePath = runLog.RunLogPath + "\\" + databaseInterface.GetDatabaseName();
                parentItemLog.SubFlowLogs.Add(itemLog);
            }

            ((IDataSource)sourceObject).LoadData(connectionObject, dataStore, reportProgressMethod, mappingPreview);

            itemLog.EndTime = DateTime.Now;

            return dataStore;
        }

        public IDatastore TransformData(DesignerItemBase transformationItem, List<IDatastore> dataStores, ReportProgressMethod reportProgressMethod)
        {
            var transformationItemConfiguration = objectResolver.LoadItemConfiguration(transformationItem.ID) as TransformationConfiguration;

            IConnection transformationConnectionObject = null;
            if (transformationItemConfiguration.SelectedConnectionConfigurationId.Equals(Guid.Empty) == false)
            {
                transformationConnectionObject = objectResolver.GetConnection(transformationItemConfiguration.SelectedConnectionConfigurationId);
            }
            foreach (var dataStore in dataStores)
            {
                dataStore.ApplyFilter(transformationItemConfiguration.DataFilter);
            }

            IModule transformationObject = objectResolver.GetModule(transformationItem.ID, transformationItem.ModuleDescription.ModuleType);
            if (transformationObject is IDataTransformation)
            {
                ((IDataTransformation)transformationObject).TransformData(transformationConnectionObject, null, dataStores.First(), reportProgressMethod);
                return dataStores.First();
            }
            else if(transformationObject is IDataMerge)
            {
                if(dataStores.Count !=2)
                {
                    throw new Exception("Make sure to connect exactly two incoming datasources on JoinRecords transformations!");
                }
                var mergedStore = ((IDataMerge)transformationObject).TransformData(transformationConnectionObject, null, dataStores[0], dataStores[1], reportProgressMethod);
                return mergedStore;
            }
            else
            {
                throw new Exception("Tranformation does not implement a valid Tranformation-Interface!");
            }
        }

        public IDatastore WriteToTarget(DesignerItemBase targetItem, IDatastore dataStore, ReportProgressMethod reportProgressMethod)
        {
            IModule targetModule = objectResolver.GetModule(targetItem.ID, targetItem.ModuleDescription.ModuleType);
            IConnection connectionObject = objectResolver.GetConnection(targetItem.ID);
            IDatabaseInterface databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, targetItem.ID, targetItem.ItemLabel);

            ItemLog itemLog = ItemLog.CreateNew(targetItem.ID, targetItem.ItemLabel, targetItem.ModuleDescription.ModuleType.Name, runLog.RunLogPath + "\\" + databaseInterface.GetDatabaseName());

            ((IDataTarget)targetModule).WriteData(connectionObject, databaseInterface, dataStore, reportProgressMethod);

            itemLog.EndTime = DateTime.Now;
            parentItemLog.SubFlowLogs.Add(itemLog);

            return dataStore;
        }
    }
}
