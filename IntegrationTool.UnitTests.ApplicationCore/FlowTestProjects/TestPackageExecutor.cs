using IntegrationTool.ApplicationCore;
using IntegrationTool.Flowmanagement;
using IntegrationTool.SDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.ApplicationCore.FlowTestProjects
{
    public class TestPackageExecutor
    {
        public List<ProgressReport> ItemStarts { get; set; }
        public List<ProgressReport> ProgressReports { get; set; }
        public List<ProgressReport> ItemStops { get; set; }

        private ApplicationInitializer appInitializer;

        public TestPackageExecutor(ApplicationInitializer appInitializer)
        {
            ItemStarts = new List<ProgressReport>();
            ProgressReports = new List<ProgressReport>();
            ItemStops = new List<ProgressReport>();
            this.appInitializer = appInitializer;
        }

        public void ExecutePackage(Package package, ObservableCollection<ConnectionConfigurationBase> connections)
        {
            Task.Run(() => RunPackage(package, connections, appInitializer.ModuleLoader.Modules, appInitializer.ModuleLoader.GetModuleTypeList())).Wait();
        }

        private async Task RunPackage(Package package, ObservableCollection<ConnectionConfigurationBase> connections, List<ModuleDescription> modules, Type[] extraTypes)
        {
            FlowManager flowManager = new FlowManager(connections, modules, package);
            flowManager.DesignerItemStart += flowManager_DesignerItemStart;
            flowManager.ProgressReport += flowManager_ProgressReport;
            flowManager.DesignerItemStop += flowManager_DesignerItemStop;
            RunLog runLog = new RunLog();
            await flowManager.Run(runLog);
        }

        void flowManager_DesignerItemStop(object sender, EventArgs e)
        {
            ProgressReport d = sender as ProgressReport;
            ItemStops.Add(d);
        }

        void flowManager_DesignerItemStart(object sender, EventArgs e)
        {
            ProgressReport d = sender as ProgressReport;
            ItemStarts.Add(d);
        }

        private void flowManager_ProgressReport(object sender, EventArgs e)
        {
            ProgressReport d = sender as ProgressReport;

            ProgressReports.Add(d);
        }

        public static TestPackageExecutor ExecuteProjectPackage(string projectPath, string packageName)
        {
            ApplicationInitializer appInitializer = new ApplicationInitializer();
            Project project = ProjectLoader.LoadFromPath(projectPath, appInitializer);

            TestPackageExecutor testExecutor = new TestPackageExecutor(appInitializer);
            testExecutor.ExecutePackage(project.Packages.Where(t => t.DisplayName == packageName).First(), project.Connections);

            return testExecutor;
        }
    }
}
