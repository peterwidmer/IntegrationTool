using IntegrationTool.Module.LoadFromODBC.SDK.Enums;
using IntegrationTool.Module.LoadFromODBC.UserControls;
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

namespace IntegrationTool.Module.LoadFromODBC
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private LoadFromODBCConfiguration loadFromODBCConfiguration;

        public ConfigurationWindow(LoadFromODBCConfiguration loadFromODBCConfiguration)
        {
            InitializeComponent();
            this.DataContext = this.loadFromODBCConfiguration = loadFromODBCConfiguration;
        }

        private void ddQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddQueryType.SelectedItem != null)
            {
                switch (this.loadFromODBCConfiguration.QueryType)
                {
                    case ODBCQueryType.ODBCQuery:
                        this.QueryTypeContentControl.Content = new OdbcQueryControl();
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
