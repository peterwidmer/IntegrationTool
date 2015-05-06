using IntegrationTool.ProjectDesigner.Classes;
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

namespace IntegrationTool.ProjectDesigner.MenuWindows
{
    

    public class NewProjectEventArgs : EventArgs
    {
        public WindowResult Status { get; set; }
        public Project Project { get; set; }
    }

    /// <summary>
    /// Interaction logic for NewProject.xaml
    /// </summary>
    public partial class NewProject : UserControl
    {
        public event EventHandler Closed;

        public Project Project { get; set; }
        public NewProject()
        {
            InitializeComponent();
            this.DataContext = this.Project = new Project();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if(Closed != null)
            {
                Closed(this, new NewProjectEventArgs() { Status = WindowResult.Created, Project = this.Project });
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, new NewProjectEventArgs() { Status = WindowResult.Canceled });
            }
        }

        private void btnOpenFolderSelection_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Project.ProjectFolder = folderBrowserDialog.SelectedPath;
                tbProjectFolder.Text = Project.ProjectFolder;
            }
        }

    }
}
