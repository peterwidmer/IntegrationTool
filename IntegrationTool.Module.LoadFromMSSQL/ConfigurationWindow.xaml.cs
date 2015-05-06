using IntegrationTool.Module.LoadFromMSSQL.SDK.Enums;
using IntegrationTool.Module.LoadFromMSSQL.UserControls;
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

namespace IntegrationTool.Module.LoadFromMSSQL
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private LoadFromMSSQLConfiguration loadFromSqlConfiguration;

        public ConfigurationWindow(LoadFromMSSQLConfiguration loadFromSqlConfiguration)
        {
            InitializeComponent();
            this.DataContext = this.loadFromSqlConfiguration = loadFromSqlConfiguration;
        }

        private void ddQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ddQueryType.SelectedItem != null)
            {
                switch(this.loadFromSqlConfiguration.QueryType)
                {
                    case QueryType.SqlQuery:
                        this.QueryTypeContentControl.Content = new SqlQueryControl();
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
