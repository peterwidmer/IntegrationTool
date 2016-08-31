using IntegrationTool.DBAccess;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Diagram;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ItemExecution
    {
        private ItemWorker itemWorker;
        private ObjectResolver objectResolver;
        private RunLog runLog;

        public ItemExecution(ItemWorker itemWorker, ObjectResolver objectResolver, RunLog runLog)
        {
            this.itemWorker = itemWorker;
            this.objectResolver = objectResolver;
            this.runLog = runLog;
        }

        public void Execute()
        {
            var designerItem = itemWorker.DesignerItem;

            IModule stepModule = objectResolver.GetModule(designerItem.ID, designerItem.ModuleDescription.ModuleType);
            IConnection connectionObject = null;
            if (designerItem.ModuleDescription.Attributes.RequiresConnection)
            {
                connectionObject = objectResolver.GetConnection(designerItem.ID);
            }
            IDatabaseInterface databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, designerItem.ID, designerItem.ItemLabel);

            ((IStep)stepModule).Execute(connectionObject, databaseInterface, ReportProgressMethod);
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
            if (this.itemWorker != null)
            {
                this.itemWorker.BackgroundWorker.ReportProgress(100, new ProgressReport() { DesignerItem = itemWorker.DesignerItem, State = ItemEvent.ProgressReport, Message = progress.Message });
            }
        }
    }
}
