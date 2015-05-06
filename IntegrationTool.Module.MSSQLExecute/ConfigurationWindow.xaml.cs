using IntegrationTool.Module.MSSQLExecute.UserControls;
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

namespace IntegrationTool.Module.MSSQLExecute
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private MSSQLExecuteConfiguration mssqlExecuteConfiguraration;

        public ConfigurationWindow(MSSQLExecuteConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = this.mssqlExecuteConfiguraration = configuration;
        }

        private void ddExecutionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddExecutionType.SelectedItem != null)
            {
                switch (this.mssqlExecuteConfiguraration.ExecutionType)
                {
                    case ExecutionType.ExecuteNonQuery:
                        this.ExecutionTypeContent.Content = new ExecuteNonQueryControl();
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
