using IntegrationTool.Module.ExecuteCmd.UserControls;
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

namespace IntegrationTool.Module.ExecuteCmd
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private ExecuteCmdConfiguration executeCmdConfiguration;

        public ConfigurationWindow(ExecuteCmdConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = this.executeCmdConfiguration = configuration;
        }

        private void ddExecutionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddExecutionType.SelectedItem != null)
            {
                switch (this.executeCmdConfiguration.ExecutionType)
                {
                    case CmdExecutionType.cmd:
                        this.ExecutionTypeContent.Content = new ExecuteSingleCommand();
                        break;
                }
            }
            else
            {
                this.ExecutionTypeContent.Content = null;
            }
        }
    }
}
