using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using Microsoft.Xrm.Sdk.Metadata;
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

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.UserControls
{
    /// <summary>
    /// Interaction logic for MemberJoinMapping.xaml
    /// </summary>
    public partial class MemberJoinMapping : UserControl
    {
        private IDatastore dataObject;
        private List<EntityMetadata> entitiesMetadata;
        private WriteToDynamicsCrmMarketingListsConfiguration configuration;

        public MemberJoinMapping(WriteToDynamicsCrmMarketingListsConfiguration configuration, List<EntityMetadata> entitiesMetadata, IDatastore dataObject)
        {
            InitializeComponent();
            this.configuration = configuration;
            this.entitiesMetadata = entitiesMetadata;
            this.dataObject = dataObject;
        }

        private void ddMemberType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddMemberType.SelectedItem == null) { return; }

            InitializeMappingControl();
        }

        private void InitializeMappingControl()
        {
            listMemberJoinMapping.SourceList.Clear();
            listMemberJoinMapping.TargetList.Clear();

            string selectedEntityType = ((ComboBoxItem)ddMemberType.SelectedItem).Tag.ToString().ToLower();
            EntityMetadata entityMetadata = entitiesMetadata.Where(t => t.LogicalName == selectedEntityType).First();

            List<NameDisplayName> sourceList = this.dataObject.Metadata.GetColumnsAsNameDisplayNameList();
            foreach (NameDisplayName item in sourceList)
            {
                listMemberJoinMapping.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            List<NameDisplayName> targetList = Crm2013Wrapper.Crm2013Wrapper.GetAllAttributesOfEntity(entityMetadata);
            foreach (NameDisplayName item in targetList)
            {
                listMemberJoinMapping.TargetList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            listMemberJoinMapping.Mapping = this.configuration.ListMemberMapping;

            listMemberJoinMapping.RedrawLines();
        }
    }
}
