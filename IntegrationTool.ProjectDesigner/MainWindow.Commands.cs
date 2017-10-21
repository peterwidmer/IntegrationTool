using IntegrationTool.ApplicationCore;
using IntegrationTool.ApplicationCore.Serialization;
using IntegrationTool.Flowmanagement;
using IntegrationTool.ProjectDesigner.Classes;
using IntegrationTool.ProjectDesigner.MenuWindows;
using IntegrationTool.ProjectDesigner.Screens;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IntegrationTool.ProjectDesigner
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            
        }

        #region New Project Command Handling

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {  
                MenuWindows.NewProject newProject = new MenuWindows.NewProject();
                newProject.Closed += newProject_Closed;
                this.mainWindowContent.Content = newProject;
        }

        void newProject_Closed(object sender, EventArgs e)
        {
            NewProjectEventArgs newProjectEventArgs = e as NewProjectEventArgs;
            if(newProjectEventArgs != null)
            {
                switch(newProjectEventArgs.Status)
                {
                    case WindowResult.Canceled:
                        if(this.CurrentProject != null)
                        {
                            ShowProject();
                        }
                        else
                        {
                            this.mainWindowContent.Content = null;
                        }
                        break;

                    case WindowResult.Created:
                        if (this.CurrentProject != null && System.Windows.MessageBox.Show("Current project will be closed! Unsaved changes will be lost!",
                                                "Warning",
                                                System.Windows.MessageBoxButton.OKCancel)
                            == System.Windows.MessageBoxResult.Cancel)
                        {
                            return;
                        }

                        this.CurrentProject = newProjectEventArgs.Project;

                        SaveProject();
                        ShowProject();                        
                        break;
                }
            }
        }

        private void New_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {           
                e.CanExecute = true;
        }

        #endregion

        #region Open Project Command Handling

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK && File.Exists(openFileDialog.FileName))
            {
                OpenProject(openFileDialog.FileName);   
            }
        }

        private void OpenProject(string projectPath)
        {
            try
            {
                Type[] extraTypes = applicationInitializer.ModuleLoader.GetModuleTypeList();

                this.CurrentProject = Project.LoadFromFile(projectPath, extraTypes);
                this.CurrentProject.Initialize(applicationInitializer.ModuleLoader.Modules);
                this.CurrentProject.LoadRunLogs();

                ShowProject();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Could not open the project: " + ex.Message);
            }
        }

        private void Open_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion

        #region Save Project Command Handling

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveProject();
        }

        private void Save_Enabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.CurrentProject != null;
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        private void SaveProject()
        {
            // Store current package if in design-mode
            var packageOverview = this.mainWindowContent.Content as Screens.PackageOverview;
            if (packageOverview != null)
            {
                packageOverview.SaveDiagram();
            }
            
            Type[] extraTypes = applicationInitializer.ModuleLoader.GetModuleTypeList();

            // Save the connections to disk
            ObservableCollection<ConnectionConfigurationBase> connections = this.CurrentProject.Connections;
            string serializedConnections = ConfigurationSerializer.SerializeObject(connections, extraTypes);
            ConfigurationFileHandler.SaveStringToFile(this.CurrentProject.ProjectName + "Connections.xml",
                                          this.CurrentProject.ProjectFolder,
                                          serializedConnections);

            // Save the project to disk
            this.CurrentProject.Connections = null;
            string serializedProject = ConfigurationSerializer.SerializeObject(this.CurrentProject, extraTypes);
            ConfigurationFileHandler.SaveStringToFile(this.CurrentProject.ProjectName + ".xml",
                                                      this.CurrentProject.ProjectFolder,
                                                      serializedProject);

            this.CurrentProject.Connections = connections;

        }

        

        private void ShowProject()
        {
            Screens.ProjectOverview projectOverview = new Screens.ProjectOverview(applicationInitializer.ModuleLoader, this.CurrentProject);
            projectOverview.PackageDblClicked += projectOverview_PackageDblClicked;
            projectOverview.RunLogDblClicked += projectOverview_RunLogDblClicked;
            this.mainWindowContent.Content = projectOverview;
        }

        void projectOverview_RunLogDblClicked(object sender, EventArgs e)
        {
            LogOverview logOverview = new LogOverview((RunLog)sender, applicationInitializer.ModuleLoader.Modules);
            logOverview.BackButtonClicked += backButtonClicked;
            this.mainWindowContent.Content = logOverview;
        }

        void projectOverview_PackageDblClicked(object sender, EventArgs e)
        {
            var packageOverview = new Screens.PackageOverview(applicationInitializer.ModuleLoader, this.CurrentProject.Connections, (Package)sender);
            packageOverview.BackButtonClicked += backButtonClicked;
            packageOverview.ProgressReport += packageOverview_ProgressReport;
            this.mainWindowContent.Content = packageOverview;
        }

        void packageOverview_ProgressReport(object sender, EventArgs e)
        {
            if(this.mainWindowContent.Content as Screens.PackageOverview == null)
            {
                return;
            }
            ProgressReport d = sender as ProgressReport;

            ((Screens.PackageOverview)this.mainWindowContent.Content).PackageRunStatus.AddStatusMessage(d);
        }

        void backButtonClicked(object sender, EventArgs e)
        {
            ShowProject();
        }

    }
}
