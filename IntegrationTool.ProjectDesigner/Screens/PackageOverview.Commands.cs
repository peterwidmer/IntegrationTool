using IntegrationTool.Flowmanagement;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IntegrationTool.ProjectDesigner.Screens
{
    public partial class PackageOverview
    {
        public RoutedCommand RunPackage = new RoutedCommand();
        public RoutedCommand GoBack = new RoutedCommand();

        private bool packageIsRunning;

        public void InitializeCommands()
        {
            this.CommandBindings.Add(new CommandBinding(this.RunPackage, RunPackage_Executed, RunPackage_Enabled));
            btnRunPackage.Command = this.RunPackage;

            this.CommandBindings.Add(new CommandBinding(this.GoBack, GoBack_Executed, GoBack_Enabled));
            btnBack.Command = this.GoBack;
        }

        private void RunPackage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExecutePackageFlow();   
        }

        private void RunPackage_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !packageIsRunning;
        }

        private void GoBack_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveDiagram();
            if (BackButtonClicked != null)
            {
                BackButtonClicked(sender, e);
            }
        }

        private void GoBack_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !packageIsRunning;
        }

        private void ExecutePackageFlow()
        {
            if (mainFlowContent.Content == null)
            {
                return;
            }

            SaveDiagram();

            packageIsRunning = true;
            FlowManager flowManager = new FlowManager(this.Connections, this.moduleLoader.Modules, this.Package);

            FlowDesign.FlowDesigner mainFlowDesigner = mainFlowContent.Content as FlowDesign.FlowDesigner;
            mainFlowDesigner.MyDesigner.ExecuteFlow(flowManager);
            flowManager.DesignerItemStart += flowManager_ProgressReport;
            flowManager.ProgressReport += flowManager_ProgressReport;
            flowManager.DesignerItemStop += flowManager_ProgressReport;
            flowManager.RunCompleted += flowManager_RunCompleted;
            AdditionalInfosArea.Height = new GridLength(120);

            RunLog runLog = new RunLog();
            this.Package.ParentProject.RunLogs.Add(runLog);
            flowManager.Run(runLog);
        }

        void flowManager_RunCompleted(object sender, EventArgs e)
        {
            packageIsRunning = false;
        }

        void flowManager_ProgressReport(object sender, EventArgs e)
        {
            if (ProgressReport != null)
            {
                ProgressReport(sender, e);
            }
        }
    }
}
