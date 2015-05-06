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

namespace IntegrationTool.Module.WriteToDynamicsCrm.UserControls.Relation
{
    /// <summary>
    /// Interaction logic for RelationMappingKeyMapping.xaml
    /// </summary>
    public partial class RelationMappingKeyMapping : UserControl
    {
        public event EventHandler DeleteRelationMapping;

        private IntegrationTool.Module.WriteToDynamicsCrm.SDK.RelationMapping relationMappingConfiguration;

        public RelationMappingKeyMapping(IntegrationTool.Module.WriteToDynamicsCrm.SDK.RelationMapping relationMappingConfiguration, List<NameDisplayName> sourceList, List<NameDisplayName> targetList)
        {
            InitializeComponent();

            this.DataContext = this.relationMappingConfiguration = relationMappingConfiguration;
            InitializeMappingControl(sourceList, targetList);
        }

        private void InitializeMappingControl(List<NameDisplayName> sourceList, List<NameDisplayName> targetList)
        {
            foreach (NameDisplayName item in sourceList)
            {
                KeyMappingControl.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            foreach (NameDisplayName item in targetList)
            {
                KeyMappingControl.TargetList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            KeyMappingControl.Mapping = this.relationMappingConfiguration.Mapping;

            KeyMappingControl.RedrawLines();
        }

        private void ddMappingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(this.relationMappingConfiguration.MappingType)
            {
                case SDK.Enums.RelationMappingType.Manual:
                    this.KeyMappingControl.Visibility = System.Windows.Visibility.Visible;
                    break;

                case SDK.Enums.RelationMappingType.Automatic:
                    this.KeyMappingControl.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
        }

        private void btnDeleteRelationMapping_Click(object sender, RoutedEventArgs e)
        {
            if(DeleteRelationMapping != null && MessageBox.Show("Do you really want to delete this relationmapping?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DeleteRelationMapping(this.relationMappingConfiguration, new EventArgs());
            }
        }
    }
}
