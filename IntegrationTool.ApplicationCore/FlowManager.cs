
using IntegrationTool.ApplicationCore;
using IntegrationTool.ApplicationCore.Serialization;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Diagram;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Flowmanagement
{   
    public class FlowManager
    {
        private RunLog runLog;
        private string logBasePath;

        public event EventHandler DesignerItemStart;
        public event EventHandler DesignerItemStop;
        public event EventHandler ProgressReport;

        private IProgress<ProgressReport> progress;

        private BlockingCollection<ConnectionBase> connectionList;
        private BlockingCollection<ItemWorker> itemWorkers = new BlockingCollection<ItemWorker>();
        private ObservableCollection<ConnectionConfigurationBase> connectionConfigurations;
        private List<ModuleDescription> loadedModules;
        private Package package;
        
        public FlowManager(ObservableCollection<ConnectionConfigurationBase> connectionConfigurations, List<ModuleDescription> loadedModules, Package package)
        {
            this.connectionConfigurations = connectionConfigurations;
            this.loadedModules = loadedModules;
            this.package = package;
            
            IntegrationTool.SDK.Diagram.DiagramDeserializer deserializer = new SDK.Diagram.DiagramDeserializer(this.loadedModules, this.package.Diagram.Diagram);

            this.connectionList = deserializer.Connections;
            foreach (DesignerItemBase item in deserializer.DesignerItems)
            {
                StepConfiguration configuration = package.Configurations.Where(t => t.ConfigurationId == item.ID).FirstOrDefault() as StepConfiguration;
                ItemWorker itemWorker = InitializeItemWorker(item, configuration);
                itemWorkers.Add(itemWorker);
            }
        }

        void bgw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DesignerItemBase designerItem = (DesignerItemBase)((object[])e.Argument)[0];
            StepConfiguration configuration = (StepConfiguration)((object[])e.Argument)[1]; 

            ItemLog itemLog = ItemLog.CreateNew(designerItem.ID, designerItem.ItemLabel, designerItem.ModuleDescription.ModuleType.Name, DateTime.Now);

            ObjectResolver objectResolver = new ObjectResolver(this.package.Configurations.OfType<StepConfigurationBase>().ToList(), connectionConfigurations);

            try
            {
                if (designerItem.ModuleDescription.Attributes.ContainsSubConfiguration)
                {
                    SerializedDiagram subDiagram = this.package.SubDiagrams.Where(t => t.ParentItemId == designerItem.ID).FirstOrDefault();
                    if (subDiagram != null)
                    {
                        IntegrationTool.SDK.Diagram.DiagramDeserializer deserializer = new SDK.Diagram.DiagramDeserializer(this.loadedModules, subDiagram.Diagram);
                        SubFlowExecution subFlowExecution = new SubFlowExecution(designerItem, itemLog, objectResolver, deserializer.DesignerItems, deserializer.Connections);
                        subFlowExecution.Execute(this.runLog);                        
                    }
                }
                else
                {                    
                    ItemExecution itemExecution = new ItemExecution(designerItem, objectResolver, this.runLog);
                    itemExecution.Execute();
                }
            }
            catch(Exception ex)
            {
                itemLog.ExecutionError = ex.ToString();
            }

            itemLog.EndTime = DateTime.Now;
            lock(this.runLog.ItemLogs)
            {
                this.runLog.ItemLogs.Add(itemLog);
            }

            e.Result = itemLog;
        }

        

        private ItemWorker InitializeItemWorker(DesignerItemBase designerItem, StepConfigurationBase stepConfiguration)
        {
            ItemWorker itemWorker = new ItemWorker(designerItem, ItemState.Initialized, stepConfiguration);
            itemWorker.DesignerItem.BackgroundWorker = new System.ComponentModel.BackgroundWorker();
            itemWorker.DesignerItem.BackgroundWorker.WorkerReportsProgress = true;
            itemWorker.DesignerItem.BackgroundWorker.DoWork += bgw_DoWork;
            itemWorker.DesignerItem.BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            itemWorker.DesignerItem.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            return itemWorker;
        }

        public async Task Run(RunLog runLog)
        {
            this.runLog = runLog;
            this.runLog.StartTime = DateTime.Now;
            this.runLog.PackageId = this.package.PackageId;
            this.runLog.PackageDisplayName = this.package.DisplayName;

            // Create new folder for logs
            this.logBasePath = this.package.ParentProject.ProjectFolder + @"\logs\";
            if (Directory.Exists(this.logBasePath) == false)
            {
                Directory.CreateDirectory(this.logBasePath);
            }
            this.runLog.RunLogPath = this.logBasePath + this.runLog.RunId;
            Directory.CreateDirectory(this.runLog.RunLogPath);

            var progress = new Progress<ProgressReport>();
            progress.ProgressChanged += (s, e) =>
            {
                switch (e.State)
                {
                    case ItemEvent.Started:
                        if (DesignerItemStart != null)
                        {
                            DesignerItemStart(e, new EventArgs());
                        }
                        break;

                    case ItemEvent.ProgressReport:
                        if (ProgressReport != null)
                        {
                            ProgressReport(e, new EventArgs());
                        }
                        break;

                    case ItemEvent.StoppedNotExecuted:
                    case ItemEvent.StoppedWithError:
                    case ItemEvent.StoppedSuccessful:
                        if (DesignerItemStop != null)
                        {
                            DesignerItemStop(e, new EventArgs());
                        }
                        break;
                }
            };

            await Task.Run(() => RunInternal(progress));
        }

        private async Task RunInternal(IProgress<ProgressReport> progress)
        {
            this.progress = progress;

            await Task.Run(() =>
            {
                ItemWorker[] startDesignerItems = GetStartDesignerItems();
                ItemWorker[] endDesignerItems = GetEndDesignerItems();

                // Start starteritems
                foreach (ItemWorker item in startDesignerItems)
                {
                    ExecuteDesignerItem(item);
                }

                while (true)
                {
                    // Continously check if all items finished already
                    List<ItemWorker> unfinishedItemWorkers = itemWorkers.Where(t => t.State != ItemState.Stopped && t.State != ItemState.Error && t.State != ItemState.NotExecuted).ToList();
                    if (unfinishedItemWorkers.Count == 0)
                    {
                        break;
                    }

                    // Check if a new item can be started
                    List<ItemWorker> finishedItems = itemWorkers.Where(t => t.State == ItemState.Stopped || t.State == ItemState.Error || t.State == ItemState.NotExecuted).ToList();
                    foreach (ItemWorker itemWorker in unfinishedItemWorkers.Where(t => t.State == ItemState.Initialized))
                    {
                        // Initialize
                        bool newItemCanBeStarted = true;

                        // If not all incoming connections to the item are from finished items, the item may not be started
                        IEnumerable<ConnectionBase> incomingConnections = FlowHelper.GetIncomingConnections(itemWorker.DesignerItem.ID, connectionList);
                        foreach (ConnectionBase connection in incomingConnections)
                        {
                            if (finishedItems.Where(t => t.DesignerItem.ID == connection.SourceID).Count() == 0)
                            {
                                newItemCanBeStarted = false;
                            }
                        }                        

                        if (newItemCanBeStarted == true)
                        {
                            // Check if all incoming connection allow an execution
                            bool allowExecution_PreviousErrorTest = FlowHelper.AllowExecution_OnPreviousErrorTest(itemWorker, this.itemWorkers, this.connectionList);
                            bool allowExecution_PreviousItemNotSuccessfulOnErrorlineTest = FlowHelper.AllowExecution_PreviousStepNotSuccessfulOnErrorlineTest(itemWorker, this.itemWorkers, this.connectionList);
                            if (allowExecution_PreviousErrorTest == true && allowExecution_PreviousItemNotSuccessfulOnErrorlineTest)
                            {
                                ExecuteDesignerItem(itemWorker);
                            }
                            else
                            {
                                DoNotExecuteDesignerItem(itemWorker);
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(200);
                }

                this.runLog.EndTime = DateTime.Now;
                
                string serializedRunLog = ConfigurationSerializer.SerializeObject(runLog, new Type [] {});
                ConfigurationFileHandler.SaveStringToFile("runLog.xml", this.runLog.RunLogPath, serializedRunLog);
            });
        }

        private void ExecuteDesignerItem(ItemWorker itemWorker)
        {
            progress.Report(new ProgressReport() { State = ItemEvent.Started, DesignerItem = itemWorker.DesignerItem, Message = "Started" });

            itemWorker.State = ItemState.Running;
            itemWorker.DesignerItem.BackgroundWorker.RunWorkerAsync(
                new object [] { itemWorker.DesignerItem, itemWorker.Configuration });

        }

        private void DoNotExecuteDesignerItem(ItemWorker itemWorker)
        {
            itemWorker.State = ItemState.NotExecuted;
            progress.Report(new ProgressReport() { State = ItemEvent.StoppedNotExecuted, DesignerItem = itemWorker.DesignerItem, Message = "Not Executed" });            
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ItemLog resultItemLog = e.Result as ItemLog;

            ItemWorker itemWorker = itemWorkers.Where(t => t.DesignerItem.BackgroundWorker == (BackgroundWorker)sender).FirstOrDefault();
            itemWorker.State = resultItemLog.ExecutionSuccessful ? ItemState.Stopped : ItemState.Error;

            ProgressReport progressReport = new SDK.ProgressReport()
            {
                State = resultItemLog.ExecutionSuccessful ?
                                ItemEvent.StoppedSuccessful :
                                ItemEvent.StoppedWithError,
                DesignerItem = itemWorker.DesignerItem,
                Message = resultItemLog.ExecutionSuccessful ? "Successful" : "Error"
            };

            progress.Report(progressReport);
        }

        void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            ItemWorker backgroundWorker = itemWorkers.Where(t => t.DesignerItem.BackgroundWorker == (BackgroundWorker)sender).FirstOrDefault();

            ProgressReport currentProgress = e.UserState as ProgressReport;
            currentProgress.DesignerItem = backgroundWorker.DesignerItem;
            currentProgress.State = ItemEvent.ProgressReport;
            if (progress != null)
            {
                progress.Report(currentProgress);
            }
        }

        private ItemWorker[] GetStartDesignerItems()
        {
            // Search for DesignerItems where the designerItems is not at the end of a connectionsink
            var qStartItems = from items in itemWorkers
                              where
                                  (from connections
                                   in connectionList
                                   select connections.SinkID)
                                   .Contains(items.DesignerItem.ID) == false
                              select items;

            return qStartItems.ToArray<ItemWorker>();
        }

        private ItemWorker[] GetEndDesignerItems()
        {
            // Search for DesignerItems where the designerItems is not at the end of a connectionsink
            var qEndItems = from items in itemWorkers
                            where
                                (from connections
                                 in connectionList
                                 select connections.SourceID)
                                .Contains(items.DesignerItem.ID) == false
                            select items;

            return qEndItems.ToArray<ItemWorker>();
        }
    }
}
