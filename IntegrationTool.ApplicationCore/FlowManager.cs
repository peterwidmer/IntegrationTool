
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
        public event EventHandler RunCompleted;

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
            
            var diagramDeserializer = new DiagramDeserializer(this.loadedModules, this.package.Diagram.Diagram);

            this.connectionList = diagramDeserializer.Connections;
            foreach (DesignerItemBase item in diagramDeserializer.DesignerItems)
            {
                var stepConfiguration = package.Configurations.FirstOrDefault(t => t.ConfigurationId == item.ID) as StepConfiguration;
                var itemWorker = InitializeItemWorker(item, stepConfiguration);
                itemWorkers.Add(itemWorker);
            }
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var itemWorker = e.Argument as ItemWorker;
            var designerItem = itemWorker.DesignerItem;

            ItemLog itemLog = ItemLog.CreateNew(designerItem.ID, designerItem.ItemLabel, designerItem.ModuleDescription.ModuleType.Name, DateTime.Now);

            var objectResolver = new ObjectResolver(this.package.Configurations.OfType<StepConfigurationBase>().ToList(), connectionConfigurations);

            try
            {
                if (designerItem.ModuleDescription.Attributes.ContainsSubConfiguration)
                {
                    var subDiagram = this.package.SubDiagrams.FirstOrDefault(t => t.ParentItemId == designerItem.ID);
                    if (subDiagram != null)
                    {
                        var diagramDeserializer = new DiagramDeserializer(this.loadedModules, subDiagram.Diagram);
                        var flowGraph = new FlowGraph(diagramDeserializer.DesignerItems, diagramDeserializer.Connections);
                        var subFlowExecution = new SubFlowExecution(itemWorker, itemLog, objectResolver, flowGraph);
                        subFlowExecution.Execute(this.runLog);                        
                    }
                }
                else
                {                    
                    ItemExecution itemExecution = new ItemExecution(itemWorker, objectResolver, this.runLog);
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
            designerItem.State = ItemState.Initialized;

            ItemWorker itemWorker = new ItemWorker(designerItem, stepConfiguration);
            itemWorker.BackgroundWorker = new BackgroundWorker();
            itemWorker.BackgroundWorker.WorkerReportsProgress = true;
            itemWorker.BackgroundWorker.DoWork += bgw_DoWork;
            itemWorker.BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            itemWorker.BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            return itemWorker;
        }

        public async Task Run(RunLog runLog)
        {
            this.runLog = runLog;
            this.runLog.StartTime = DateTime.Now;
            this.runLog.PackageId = this.package.PackageId;
            this.runLog.PackageDisplayName = this.package.DisplayName;

            // Create new folder for logs
            this.logBasePath = this.package.ParentProject.ProjectFolder.Trim('\\') + @"\logs\";
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
                        DesignerItemStart?.Invoke(e, new EventArgs());
                        break;

                    case ItemEvent.ProgressReport:
                        ProgressReport?.Invoke(e, new EventArgs());
                        break;

                    case ItemEvent.StoppedNotExecuted:
                    case ItemEvent.StoppedWithError:
                    case ItemEvent.StoppedSuccessful:
                        DesignerItemStop?.Invoke(e, new EventArgs());
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
                    List<ItemWorker> unfinishedItemWorkers = itemWorkers.Where(t => t.DesignerItem.State != ItemState.Stopped && t.DesignerItem.State != ItemState.Error && t.DesignerItem.State != ItemState.NotExecuted).ToList();
                    if (unfinishedItemWorkers.Count == 0)
                    {
                        RunCompleted?.Invoke(this, new EventArgs());
                        break;
                    }

                    // Check if a new item can be started
                    List<ItemWorker> finishedItems = itemWorkers.Where(t => t.DesignerItem.State == ItemState.Stopped || t.DesignerItem.State == ItemState.Error || t.DesignerItem.State == ItemState.NotExecuted).ToList();
                    foreach (ItemWorker itemWorker in unfinishedItemWorkers.Where(t => t.DesignerItem.State == ItemState.Initialized))
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
                            bool allowExecution_activeItem = itemWorker.Configuration.Status == StepExecutionStatus.Active;

                            if (allowExecution_PreviousErrorTest && allowExecution_PreviousItemNotSuccessfulOnErrorlineTest && allowExecution_activeItem)
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

            itemWorker.DesignerItem.State = ItemState.Running;
            itemWorker.BackgroundWorker.RunWorkerAsync(itemWorker);
        }

        private void DoNotExecuteDesignerItem(ItemWorker itemWorker)
        {
            itemWorker.DesignerItem.State = ItemState.NotExecuted;
            progress.Report(new ProgressReport() { State = ItemEvent.StoppedNotExecuted, DesignerItem = itemWorker.DesignerItem, Message = "Not Executed" });            
        }

        void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ItemLog resultItemLog = e.Result as ItemLog;

            ItemWorker itemWorker = itemWorkers.FirstOrDefault(t => t.BackgroundWorker == (BackgroundWorker)sender);
            itemWorker.DesignerItem.State = resultItemLog.ExecutionSuccessful ? ItemState.Stopped : ItemState.Error;

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
            ItemWorker backgroundWorker = itemWorkers.FirstOrDefault(t => t.BackgroundWorker == (BackgroundWorker)sender);

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
