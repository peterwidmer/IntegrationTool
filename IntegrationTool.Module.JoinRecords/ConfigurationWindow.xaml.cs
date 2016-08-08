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

namespace IntegrationTool.Module.JoinRecords
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private JoinRecordsConfiguration configuration;
        private IDatastore dataObject;

        public ConfigurationWindow(JoinRecordsConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.DataContext = this.configuration = configuration;
            this.dataObject = dataObject;
        }
    }
}
