using IntegrationTool.SDK.Database;
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

namespace IntegrationTool.Module.WriteToMSSQL
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private IDatastore dataObject;
        private WriteToMSSQLConfiguration configuration;

        public ConfigurationWindow(WriteToMSSQLConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        private void ddTargetTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
