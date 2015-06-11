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

namespace IntegrationTool.Module.ConnectToDynamicsCrm
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        public ConfigurationWindow(ConnectToDynamicsCrmConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = configuration;
        }

        private void btnCrmConnection_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.freedevelopertutorials.com/integrationtool-connections/dynamics-crm-connection/");            
        }

    }
}
