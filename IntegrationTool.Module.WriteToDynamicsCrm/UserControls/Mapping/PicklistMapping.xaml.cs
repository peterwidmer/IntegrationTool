using IntegrationTool.SDK.GenericClasses;
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

namespace IntegrationTool.Module.WriteToDynamicsCrm.UserControls.Mapping
{
    /// <summary>
    /// Interaction logic for PicklistMapping.xaml
    /// </summary>
    public partial class PicklistMapping : UserControl
    {
        private IntegrationTool.Module.WriteToDynamicsCrm.SDK.PicklistMapping picklistMappingConfiguration;

        public PicklistMapping(IntegrationTool.Module.WriteToDynamicsCrm.SDK.PicklistMapping picklistMappingConfiguration, List<NameDisplayName> sourceList, List<NameDisplayName> targetList)
        {
            InitializeComponent();
            this.DataContext = this.picklistMappingConfiguration = picklistMappingConfiguration;
            InitializeMappingControl(sourceList, targetList);
        }

        private void InitializeMappingControl(List<NameDisplayName> sourceList, List<NameDisplayName> targetList)
        {
            PicklistMappingControl.StoreTooltipInTargetMappingInsteadOfContent = true;

            foreach (NameDisplayName item in sourceList)
            {
                PicklistMappingControl.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            foreach (NameDisplayName item in targetList)
            {
                PicklistMappingControl.TargetList.Add(new ListViewItem() { Content = item.DisplayName, ToolTip = item.Name });
                ddDefaultValue.Items.Add(new ComboBoxItem() { Content = item.DisplayName, ToolTip = item.Name, Tag = item.Name });
            }

            PicklistMappingControl.Mapping = this.picklistMappingConfiguration.Mapping;

            PicklistMappingControl.RedrawLines();
        }

        private void ddMappingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(this.picklistMappingConfiguration.MappingType)
            {
                case SDK.Enums.PicklistMappingType.Manual:
                    this.PicklistMappingControl.Visibility = System.Windows.Visibility.Visible;
                    break;

                case SDK.Enums.PicklistMappingType.Automatic:
                    this.PicklistMappingControl.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }
        }

        private void ddMappingNotFound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)ddMappingNotFound.SelectedItem).Tag.ToString() == "SetDefaultValue")
            {
                DefaultValueRow.Height = new GridLength(24);
            }
            else
            {
                DefaultValueRow.Height = new GridLength(0);
            }
        }


    }
}
