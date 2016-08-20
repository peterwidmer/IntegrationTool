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
    public class DataStream
    {
        public IDatastore DataStore { get; set; }

        private ObjectResolver objectResolver;
        private RunLog runLog;
        private ItemLog parentItemLog;

        public DataStream(IDatastore dataStore, ObjectResolver objectResolver, RunLog runLog, ItemLog parentItemLog)
        {
            this.DataStore = dataStore;
            this.objectResolver = objectResolver;
            this.runLog = runLog;
            this.parentItemLog = parentItemLog;
        }

        private void InitializeDatastore()
        {
            List<AttributeImplementation> dataConditionAttributes = AssemblyHelper.LoadAllClassesImplementingSpecificAttribute<DataConditionAttribute>(System.Reflection.Assembly.GetAssembly(typeof(DataConditionAttribute)));
            DataStore.InitializeDatastore(dataConditionAttributes);
        }

        public void LoadDataFromSource(DesignerItemBase sourceItem, ReportProgressMethod reportProgressMethod)
        {
            IModule sourceObject = objectResolver.GetModule(sourceItem.ID, sourceItem.ModuleDescription.ModuleType);
            IConnection connectionObject = objectResolver.GetConnection(sourceItem.ID);
            if (connectionObject == null)
            {
                if (runLog != null)
                {
                    string label = String.IsNullOrEmpty(sourceItem.ItemLabel) ? "-- No Label --" : sourceItem.ItemLabel;
                    throw new Exception("No connection was selected for the source '" + label + "'.");
                }
                this.DataStore = null;
            }

            ItemLog itemLog = ItemLog.CreateNew(sourceItem.ID, sourceItem.ItemLabel, sourceItem.ModuleDescription.ModuleType.Name, null);

            if (runLog != null)
            {
                var databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, sourceItem.ID, sourceItem.ItemLabel);
                itemLog.DatabasePath = runLog.RunLogPath + "\\" + databaseInterface.GetDatabaseName();
                parentItemLog.SubFlowLogs.Add(itemLog);
            }

            ((IDataSource)sourceObject).LoadData(connectionObject, this.DataStore, reportProgressMethod);

            itemLog.EndTime = DateTime.Now;
        }

        public void TransformData(DesignerItemBase transformationItem, ReportProgressMethod reportProgressMethod)
        {
            var transformationItemConfiguration = objectResolver.LoadItemConfiguration(transformationItem.ID) as TransformationConfiguration;

            IConnection transformationConnectionObject = null;
            if (transformationItemConfiguration.SelectedConnectionConfigurationId.Equals(Guid.Empty) == false)
            {
                transformationConnectionObject = objectResolver.GetConnection(transformationItemConfiguration.SelectedConnectionConfigurationId);
            }
            this.DataStore.ApplyFilter(transformationItemConfiguration.DataFilter);

            IModule transformationObject = objectResolver.GetModule(transformationItem.ID, transformationItem.ModuleDescription.ModuleType);
            ((IDataTransformation)transformationObject).TransformData(transformationConnectionObject, null, this.DataStore, reportProgressMethod);
        }

        public void WriteToTarget(DesignerItemBase targetItem, ReportProgressMethod reportProgressMethod)
        {
            IModule targetModule = objectResolver.GetModule(targetItem.ID, targetItem.ModuleDescription.ModuleType);
            IConnection connectionObject = objectResolver.GetConnection(targetItem.ID);
            IDatabaseInterface databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, targetItem.ID, targetItem.ItemLabel);

            ItemLog itemLog = ItemLog.CreateNew(targetItem.ID, targetItem.ItemLabel, targetItem.ModuleDescription.ModuleType.Name, runLog.RunLogPath + "\\" + databaseInterface.GetDatabaseName());

            ((IDataTarget)targetModule).WriteData(connectionObject, databaseInterface, this.DataStore, reportProgressMethod);

            itemLog.EndTime = DateTime.Now;
            parentItemLog.SubFlowLogs.Add(itemLog);
        }
    }
}
