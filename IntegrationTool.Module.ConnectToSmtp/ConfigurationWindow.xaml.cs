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

namespace IntegrationTool.Module.ConnectToSmtp
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private ConnectToSmtpConfiguration connectToSmtpConfiguration;

        public ConfigurationWindow(ConnectToSmtpConfiguration connectToSmtpConfiguration)
        {
            InitializeComponent();

            this.DataContext = this.connectToSmtpConfiguration = connectToSmtpConfiguration;
        }
    }
}
