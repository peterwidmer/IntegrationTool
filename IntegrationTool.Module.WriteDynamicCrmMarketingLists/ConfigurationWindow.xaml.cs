using IntegrationTool.Module.WriteDynamicCrmMarketingLists.UserControls;
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

        private List<EntityMetadata> entitiesMetadata;

        public ConfigurationWindow(WriteToDynamicsCrmMarketingListsConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        private void ddJoinType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetListJoinType();
        }

        private void SetListJoinType()
        {
            if (this.entitiesMetadata == null) { return; }

            if (ddJoinType.SelectedItem == null)
            {
                ListMappingContent.Content = null;
            }
            switch (this.configuration.JoinList)
            {
                case MarketinglistJoinType.Manual:
                    ListMappingContent.Content = new UserControls.ListMappingManual();
                    rowJoinType.Height = new GridLength(27, GridUnitType.Pixel);
                    break;

                case MarketinglistJoinType.Join:
                    ListMappingContent.Content = new UserControls.ListMappingJoin(this.configuration, entitiesMetadata.Where(t => t.LogicalName == "list").First(), this.dataObject);
                    rowJoinType.Height = new GridLength(100, GridUnitType.Star);
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
            
            var marketingListMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.orgServiceInstance, "list");
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
            this.entitiesMetadata = (List<EntityMetadata>)((object[])e.Result)[0];
            ddJoinType.IsEnabled = true;
            ddJoinUnsuccessful.IsEnabled = true;
            SetListJoinType();
            MemberMappingContent.Content = new MemberJoinMapping(this.configuration, this.entitiesMetadata, this.dataObject);
        }
    }
}
