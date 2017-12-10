using IntegrationTool.Module.ConnectToSharepoint.UserControls;
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

namespace IntegrationTool.Module.ConnectToSharepoint
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private ConnectToSharepointConfiguration configuration;

        public ConfigurationWindow(ConnectToSharepointConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = this.configuration = configuration;
        }

        private void ddAuthenticationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddAuthenticationType.SelectedItem != null)
            {
                switch (this.configuration.AuthenticationType)
                {
                    case SharepointAuthenticationType.OnPremise:
                        this.AuthenticationTypeContent.Content = new OnPremiseAuthenticationControl();
                        break;

                    case SharepointAuthenticationType.SharepointOnline:
                        this.AuthenticationTypeContent.Content = new SharepointOnlineAuthentication();
                        break;
                }
            }
            else
            {
                this.AuthenticationTypeContent.Content = null;
            }
        }
    }
}
