using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTool.ApplicationCore;
using IntegrationTool.Flowmanagement;
using IntegrationTool.SDK;
using static System.Configuration.ConfigurationSettings;

namespace IntegrationTool.WindowsServiceProjectStarter
{
    public class MigrationRunner
    {
        private readonly string[] _campaignIds;
        private readonly AutoResetEvent _shouldShutdownEventHandle;
        private readonly AutoResetEvent _isShutdownEventHandle;

        private EventLog _eventLog;
        public MigrationRunner()
        {
            _eventLog = new EventLog();
            if (!EventLog.SourceExists("MigrationService"))
            {
                EventLog.CreateEventSource(
                    "MigrationService", "Migration ServiceLog");
            }
            _eventLog.Source = "MigrationService";
            _eventLog.Log = "Migration ServiceLog";


            _shouldShutdownEventHandle = new AutoResetEvent(false);
            _isShutdownEventHandle = new AutoResetEvent(false);
        }

        public void Start()
        {
            try
            {
                var runningThread = new Thread(Run);
                runningThread.Start();
            }
            catch (Exception e)
            {
                _eventLog.WriteEntry(e.Message);
                _eventLog.WriteEntry(e.StackTrace);
                throw;
            }

        }

        private void Run()
        {
            _eventLog.WriteEntry("Migration Service started");

            string projectPath = AppSettings.Get("ProjectPath");
            string packages = AppSettings.Get("Packages");

            _eventLog.WriteEntry($"ProjectPath {projectPath}");
            _eventLog.WriteEntry($"Packages {packages}");

            ApplicationInitializer appInitializer = new ApplicationInitializer();
            Project project = ProjectLoader.LoadFromPath(projectPath, appInitializer);

            Type[] extraTypes = appInitializer.ModuleLoader.GetModuleTypeList();
            _eventLog.WriteEntry("Starting execution of project " + project.ProjectName + "\n");

            string[] packagesToRun = String.IsNullOrEmpty(packages) ?
                project.Packages.Select(t => t.DisplayName).ToArray<string>() :
                packagesToRun = packages.Split(',');

            List<Package> packagesToExecute = new List<Package>();
            foreach (string packageName in packagesToRun)
            {
                if (_shouldShutdownEventHandle.WaitOne(0))
                {
                    _isShutdownEventHandle.Set();
                    return;
                }

                Package package = project.Packages.Where(t => t.DisplayName == packageName).FirstOrDefault();
                if (package == null)
                {
                    _eventLog.WriteEntry("Package " + packageName + " does not exist in project " + project.ProjectName);
                    Environment.Exit(1);
                }

                packagesToExecute.Add(package);
            }

            foreach (Package package in packagesToExecute)
            {
                if (_shouldShutdownEventHandle.WaitOne(0))
                {
                    _isShutdownEventHandle.Set();
                    return;
                }

                Task.Run(() => RunPackage(package, project.Connections, appInitializer.ModuleLoader.Modules, extraTypes)).Wait();
            }

            _eventLog.WriteEntry("\nExecution finished");

            _shouldShutdownEventHandle.Set();
        }

        public EventWaitHandle Shutdown()
        {
            _shouldShutdownEventHandle.Set();
            return _isShutdownEventHandle;
        }

        static async Task RunPackage(Package package, ObservableCollection<ConnectionConfigurationBase> connections, List<ModuleDescription> modules, Type[] extraTypes)
        {
            FlowManager flowManager = new FlowManager(connections, modules, package);
            flowManager.ProgressReport += flowManager_ProgressReport;

            RunLog runLog = new RunLog();
            await flowManager.Run(runLog);
        }

        static void flowManager_ProgressReport(object sender, EventArgs e)
        {
            EventLog eventLog = new EventLog();
            if (!EventLog.SourceExists("MigrationService"))
            {
                EventLog.CreateEventSource(
                    "MigrationService", "Migration ServiceLog");
            }
            eventLog.Source = "MigrationService";
            eventLog.Log = "Migration ServiceLog";

            ProgressReport d = sender as ProgressReport;
            
            string message = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\t" + d.DesignerItem.ItemLabel + "\t" + d.Message;
            eventLog.WriteEntry(message);
        }
    }
}
