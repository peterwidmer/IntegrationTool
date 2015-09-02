using IntegrationTool.Module.LoadFromDynamicsCrm.UserControls;
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

namespace IntegrationTool.Module.LoadFromDynamicsCrm
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private LoadFromDynamicsCrmConfiguration loadFromDynamicsCrmConfiguration;

        public ConfigurationWindow(LoadFromDynamicsCrmConfiguration loadFromDynamicsCrmConfiguration)
        {
            InitializeComponent();
            this.DataContext = this.loadFromDynamicsCrmConfiguration = loadFromDynamicsCrmConfiguration;
        }

        private void ddQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddQueryType.SelectedItem != null)
            {
                switch (this.loadFromDynamicsCrmConfiguration.QueryType)
                {
                    case DynamicsCrmQueryType.ExecuteFetchXml:
                        this.QueryTypeContentControl.Content = new FetchXmlControl();
                        break;
                }
            }
            else
            {
                this.QueryTypeContentControl.Content = null;
            }
        }
    }
}
