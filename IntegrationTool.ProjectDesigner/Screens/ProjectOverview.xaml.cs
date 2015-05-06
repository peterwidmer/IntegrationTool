using IntegrationTool.ApplicationCore;
using IntegrationTool.ApplicationCore.Serialization;
using IntegrationTool.ProjectDesigner.MenuWindows;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrationTool.ProjectDesigner.Screens
{
    /// <summary>
    /// Interaction logic for ProjectOverview.xaml
    /// </summary>
    public partial class ProjectOverview : UserControl
    {
        public event EventHandler PackageDblClicked;
        public event EventHandler RunLogDblClicked;

        public Project Project { get; set; }

        private ModuleLoader moduleLoader;

        public ProjectOverview(ModuleLoader moduleLoader, Project project)
        {
            InitializeComponent();
            this.DataContext = this.Project = project;
            this.moduleLoader = moduleLoader;
        }

        private void btnAddPackage_Click(object sender, RoutedEventArgs e)
        {
            NewPackage newPackage = new NewPackage();
            newPackage.Closing += newPackage_Closing;
            newPackage.ShowDialog();
        }

        void newPackage_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NewPackage newPackage = (NewPackage)sender;
            if(newPackage.Status == Classes.WindowResult.Created)
            {
                newPackage.Package.PackageId = Guid.NewGuid();
                newPackage.Package.ParentProject = this.Project;
                this.Project.Packages.Add(newPackage.Package);
            }
        }

        private void lbPackages_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Package package = ((ListBox)sender).SelectedItem as Package;
            if (package != null && PackageDblClicked != null)
            {                
                PackageDblClicked(package, e);
            }
        }

        private void btnAddConnection_Click(object sender, RoutedEventArgs e)
        {
            NewConnection newConnection = new NewConnection(this.moduleLoader.Modules);
            ShowNewConnectionWindow(newConnection);
        }

        void newConnection_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NewConnection newConnection = (NewConnection)sender;
            switch(newConnection.Status)
            {
                case Classes.WindowResult.Created:
                    this.Project.Connections.Add(newConnection.ConnectionConfiguration);
                    break;

                case Classes.WindowResult.Canceled:
                    if (newConnection.ConnectionConfiguration == null || newConnection.OriginalConnectionConfiguration == null)
                    {
                        return;
                    }

                    object revertedConfiguration = ConfigurationSerializer.DeserializeObject(
                                                    newConnection.OriginalConnectionConfiguration,
                                                    newConnection.ConnectionConfiguration.GetType(),
                                                    moduleLoader.GetModuleTypeList()
                                                    );
                    
                    ((ConnectionConfigurationBase)revertedConfiguration).ModuleDescription = newConnection.ConnectionConfiguration.ModuleDescription;

                    int indexConfiguration = this.Project.Connections.IndexOf(newConnection.ConnectionConfiguration);
                    this.Project.Connections[indexConfiguration] = revertedConfiguration as ConnectionConfigurationBase;
                    break;
            }
        }

        private void lbConnections_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConnectionConfigurationBase connectionConfiguration = ((ListBox)sender).SelectedItem as ConnectionConfigurationBase;
            if (connectionConfiguration != null)
            {
                string orginalConnectionConfiguration = ConfigurationSerializer.SerializeObject(connectionConfiguration, this.moduleLoader.GetModuleTypeList());
                NewConnection newConnection = new NewConnection(this.moduleLoader.Modules, connectionConfiguration, orginalConnectionConfiguration);
                ShowNewConnectionWindow(newConnection);
            }
        }

        private void ShowNewConnectionWindow(NewConnection newConnection)
        {
            newConnection.Closing += newConnection_Closing;            
            newConnection.ShowDialog();
        }

        private void lbConnections_KeyUp(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Delete:
                    if (lbConnections.SelectedItem != null)
                    {
                        ConnectionConfigurationBase connectionConfiguration = ((ListBox)sender).SelectedItem as ConnectionConfigurationBase;

                        if (MessageBox.Show("Do you really want to delete the connection \"" + connectionConfiguration.Name + "\"?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            this.Project.Connections.Remove(connectionConfiguration);
                        }
                    }
                    break;
            }
        }

        private void lbPackages_KeyUp(object sender, KeyEventArgs e)
        {
            if(lbPackages.SelectedItem != null)
            {
                Package package = ((ListBox)sender).SelectedItem as Package;
                if (MessageBox.Show("Do you really want to delete the package \"" + package.DisplayName + "\"?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Project.Packages.Remove(package);
                }
            }
        }

        private void lbLogs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RunLog runLog = ((ListBox)sender).SelectedItem as RunLog;
            if (runLog != null && RunLogDblClicked != null)
            {
                RunLogDblClicked(runLog, e);
            }
        }

 
    }
}
