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

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl, IConnectionChanged
    {
        private CrmConnection crmConnection;
        private OrganizationService orgServiceInstance;

        private IDatastore dataObject;
        private WriteToDynamicsCrmMarketingListsConfiguration configuration;

        public ConfigurationWindow(WriteToDynamicsCrmMarketingListsConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        private void ddJoinType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ddJoinType.SelectedItem == null)
            {
                ListMappingContent.Content = null;
            }
            switch(this.configuration.JoinList)
            {
                case MarketinglistJoinType.Manual:
                    ListMappingContent.Content = new UserControls.ListMappingManual();
                    break;

                case MarketinglistJoinType.Join:
                    ListMappingContent.Content = new UserControls.ListMappingJoin();
                    break;
            }
        }

        public void ConnectionChanged(IConnection connection)
        {
            ListMappingContent.Content = new LoadingControl();
            MemberMappingContent.Content = new LoadingControl();

            this.crmConnection = connection.GetConnection() as CrmConnection;
            this.orgServiceInstance = new OrganizationService(crmConnection);

            BackgroundWorker bgwConnectionChanged = new BackgroundWorker();
            bgwConnectionChanged.DoWork += bgwConnectionChanged_DoWork;
            bgwConnectionChanged.RunWorkerCompleted += bgwConnectionChanged_RunWorkerCompleted;
            bgwConnectionChanged.RunWorkerAsync();      
        }        

        void bgwConnectionChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            List<EntityMetadata> entitiesMetadata = new List<EntityMetadata>();
            
            var marketingListMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.orgServiceInstance, "marketinglist");
            entitiesMetadata.Add(marketingListMetadata);

            var accountMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.orgServiceInstance, "account");
            entitiesMetadata.Add(accountMetadata);

            var contactMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.orgServiceInstance, "contact");
            entitiesMetadata.Add(contactMetadata);

            var leadMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.orgServiceInstance, "lead");
            entitiesMetadata.Add(leadMetadata);

            e.Result = new object[] { entitiesMetadata };
        }

        void bgwConnectionChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ddJoinType.IsEnabled = true;
            ddJoinType.IsEnabled = true;
        }
    }
}
