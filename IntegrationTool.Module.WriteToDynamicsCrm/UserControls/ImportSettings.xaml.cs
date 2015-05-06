using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK.GenericClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace IntegrationTool.Module.WriteToDynamicsCrm.UserControls
{
    /// <summary>
    /// Interaction logic for ExistingCheck.xaml
    /// </summary>
    public partial class ImportSettings : UserControl
    {
        public ObservableCollection<NameDisplayName> AvailablePrimaryKeyAttributes { get; set; }
        private WriteToDynamicsCrmConfiguration Configuration { get; set; }

        public ImportSettings(WriteToDynamicsCrmConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = this.Configuration = configuration;
            AvailablePrimaryKeyAttributes = new ObservableCollection<NameDisplayName>();
            ddAvailablePrimaryKeyAttributes.ItemsSource = AvailablePrimaryKeyAttributes;
            lbSelectedPrimaryKeyAttributes.ItemsSource = this.Configuration.PrimaryKeyAttributes;
        }

        private void btnAddPrimaryKey_Click(object sender, RoutedEventArgs e)
        {
            NameDisplayName selectedPrimaryKey = ddAvailablePrimaryKeyAttributes.SelectedItem as NameDisplayName;
            if(selectedPrimaryKey != null)
            {
                this.Configuration.PrimaryKeyAttributes.Add(selectedPrimaryKey.Name);
            }
        }

        private void ddImportMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.Configuration.ImportMode)
            {
                case ImportMode.None:
                case ImportMode.Create:
                    this.Configuration.MultipleFoundMode = MultipleFoundMode.None;
                    ddMultipleFoundMode.IsEnabled = false;
                    break;

                default:
                    ddMultipleFoundMode.IsEnabled = true;
                    break;
            }
        }

        private void lbSelectedPrimaryKeyAttributes_KeyUp(object sender, KeyEventArgs e)
        {
            if (lbSelectedPrimaryKeyAttributes.SelectedItem != null)
            {
                string primaryKey = ((ListBox)sender).SelectedItem.ToString();
                if (MessageBox.Show("Do you really want to delete the primarykey \"" + primaryKey + "\"?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Configuration.PrimaryKeyAttributes.Remove(primaryKey);
                }
            }
        }
    }
}
