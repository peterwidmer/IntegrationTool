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
        private DesignerItemBase stepItem;
        private ObjectResolver objectResolver;
        private RunLog runLog;

        public ItemExecution(DesignerItemBase stepItem, ObjectResolver objectResolver, RunLog runLog)
        {
            this.stepItem = stepItem;
            this.objectResolver = objectResolver;
            this.runLog = runLog;
        }

        public void Execute()
        {
            IModule stepModule = objectResolver.GetModule(this.stepItem.ID, this.stepItem.ModuleDescription.ModuleType);
            IConnection connectionObject = null;
            if (this.stepItem.ModuleDescription.Attributes.RequiresConnection)
            {
                connectionObject = objectResolver.GetConnection(this.stepItem.ID);
            }
            IDatabaseInterface databaseInterface = SqliteWrapper.GetSqliteWrapper(runLog.RunLogPath, this.stepItem.ID, this.stepItem.ItemLabel);

            ((IStep)stepModule).Execute(connectionObject, databaseInterface, ReportProgressMethod);
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
            if (this.stepItem != null)
            {
                this.stepItem.BackgroundWorker.ReportProgress(100, new ProgressReport() { DesignerItem = this.stepItem, State = ItemEvent.ProgressReport, Message = progress.Message });
            }
        }
    }
}
