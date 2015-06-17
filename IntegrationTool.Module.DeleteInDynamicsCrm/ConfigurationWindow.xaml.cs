using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericControls;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private CrmConnection crmConnection;
        private OrganizationService orgServiceInstance;

        private IDatastore dataObject;
        private DeleteInDynamicsCrmConfiguration configuration;

        private EntityMetadata entityMetadata;

        public ConfigurationWindow(DeleteInDynamicsCrmConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        public void ConnectionChanged(IConnection connection)
        {
            MainContent.Content = new LoadingControl();

            this.crmConnection = connection.GetConnection() as CrmConnection;
            this.orgServiceInstance = new OrganizationService(crmConnection);

            BackgroundWorker bgwConnectionChanged = new BackgroundWorker();
            bgwConnectionChanged.DoWork += bgwConnectionChanged_DoWork;
            bgwConnectionChanged.RunWorkerCompleted += bgwConnectionChanged_RunWorkerCompleted;
            bgwConnectionChanged.RunWorkerAsync();
        }

        void bgwConnectionChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            var entityMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.orgServiceInstance, "list");

            e.Result = new object[] { entityMetadata };
        }

        void bgwConnectionChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.entityMetadata = (EntityMetadata)((object[])e.Result)[0];
        }
    }
}
