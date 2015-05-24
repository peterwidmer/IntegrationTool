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

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
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
    }
}
