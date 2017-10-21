using IntegrationTool.ApplicationCore;
using IntegrationTool.ProjectDesigner.MenuWindows;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace IntegrationTool.ProjectDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationInitializer applicationInitializer;
        private Project CurrentProject = null;

        private ObservableCollection<MyObject> _recentFilesList = new ObservableCollection<MyObject>();
        public ObservableCollection<MyObject> RecentFilesList
        {
            get { return _recentFilesList; }
            set { _recentFilesList = value; }
        }

        public MainWindow(ApplicationInitializer applicationInitializer)
        {
            InitializeComponent();

            this.applicationInitializer = applicationInitializer;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed, New_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed, Open_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, Close_Executed));

            //this.RecentFilesList.Add(new MyObject() { Title = "Test 1" });
            //this.RecentFilesList.Add(new MyObject() { Title = "Test 2" });
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void menuHelp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.freedevelopertutorials.com/integrationtool-tutorial/");
        }
    }

    public class MyObject
    {
        public string Title { get; set; }
    }
}
