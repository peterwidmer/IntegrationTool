using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client.Services;
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

namespace IntegrationTool.Module.WriteToDynamicsCrmN2N
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private OrganizationService orgServiceInstance;

        private IDatastore dataObject;
        private WriteToDynamicsCrmN2NConfiguration configuration;

        public ConfigurationWindow(WriteToDynamicsCrmN2NConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }
    }
}
