using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Logging;
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
    /// Interaction logic for LogOverview.xaml
    /// </summary>
    public partial class LogOverview : UserControl
    {
        public event EventHandler BackButtonClicked;
        private RunLog runLog;
        private List<ModuleDescription> moduleDescriptions;
        public LogOverview(RunLog runLog, List<ModuleDescription> moduleDescriptions)
        {
            InitializeComponent();
            this.DataContext = this.runLog = runLog;
            this.moduleDescriptions = moduleDescriptions;
            this.LogLeader.Visibility = System.Windows.Visibility.Hidden;

            InitializeLogMenu();
        }

        private void InitializeLogMenu()
        {
            foreach(ItemLog itemLog in this.runLog.ItemLogs)
            {
                TreeViewItem menuItem = AddItemLogMenuItem(itemLog);
                logMenu.Items.Add(menuItem);
            }
        }

        private TreeViewItem AddItemLogMenuItem(ItemLog itemLog)
        {
            TreeViewItem menuItem = new TreeViewItem()
            {
                Header = itemLog.DesignerItemDisplayName,
                Tag = itemLog,
                Foreground = new SolidColorBrush(itemLog.ExecutionSuccessful ? Colors.Black : Colors.Red)
            };

            foreach (ItemLog subItemLog in itemLog.SubFlowLogs)
            {
                TreeViewItem subMenuItem = AddItemLogMenuItem(subItemLog);
                menuItem.Items.Add(subMenuItem);
            }

            return menuItem;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (BackButtonClicked != null)
            {
                BackButtonClicked(sender, e);
            }
        }

        private void logMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if(logMenu.SelectedItem != null)
            {
                TreeViewItem menuItem = logMenu.SelectedItem as TreeViewItem;
                ItemLog itemLog = menuItem.Tag as ItemLog;

                this.LogLeader.Visibility = System.Windows.Visibility.Visible;
                this.LogLeader.SetItemLog(itemLog);
                this.LogLeader.SetDetailedLogVisibility(Visibility.Hidden);

                if (itemLog.ExecutionSuccessful)
                {
                    ModuleDescription moduleDescription = moduleDescriptions.Where(t => t.ModuleType.Name == itemLog.ModuleDescriptionName).FirstOrDefault();
                    ILogRendering module = Activator.CreateInstance(moduleDescription.ModuleType) as ILogRendering;
                    if (module != null)
                    {
                        string databaseName = itemLog.DesignerItemId.ToString() + "_" + itemLog.DesignerItemDisplayName.ToString().Replace(" ", "_") + ".db";
                        SqliteWrapper sqliteWrapper = new SqliteWrapper(runLog.RunLogPath, databaseName);

                        this.LogLeader.SetDetailedLogVisibility(Visibility.Visible);
                        logContent.Content = module.RenderLogWindow(sqliteWrapper);
                    }
                    else
                    {
                        logContent.Content = null;
                    }
                }
                else 
                {
                    MessageControl messageControl = new MessageControl("Unexpected error occurred", itemLog.ExecutionError); // TODO Show execution error in logcontent
                    messageControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    messageControl.SetMessageHeight(800);
                    logContent.Content = messageControl;
                }
            }
        }
    }
}
