using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
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

namespace IntegrationTool.Module.JoinRecords
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private JoinRecordsConfiguration configuration;
        private IDatastore sourceDatastore;
        private IDatastore targetDatastore;

        public ConfigurationWindow(JoinRecordsConfiguration configuration, IDatastore sourceDatastore, IDatastore targetDatastore)
        {
            InitializeComponent();
            this.DataContext = this.configuration = configuration;
            this.sourceDatastore = sourceDatastore;
            this.targetDatastore = targetDatastore;

            InitializeMappingControl(configuration.JoinMapping);
        }

        private void InitializeMappingControl(List<DataMappingControl.DataMapping> mapping)
        {
            List<NameDisplayName> sourceList = this.sourceDatastore.Metadata.GetColumnsAsNameDisplayNameList();
            foreach (NameDisplayName item in sourceList)
            {
                joinMapping.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            List<NameDisplayName> targetList = this.targetDatastore.Metadata.GetColumnsAsNameDisplayNameList();
            foreach (NameDisplayName item in targetList)
            {
                joinMapping.TargetList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            joinMapping.Mapping = mapping;

            joinMapping.RedrawLines();
        }

        private void ddInputSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender as ComboBox != null)
            {
                var ddColumns = ((Grid)((ComboBox)sender).Parent).Children.OfType<ComboBox>().Where(t => t.Name == "ddColumns").First();
                var ddDataStream = sender as ComboBox;
                if(ddDataStream.SelectedItem != null && (string)((ComboBoxItem)ddDataStream.SelectedItem).Tag == "Left")
                {
                    ddColumns.ItemsSource = sourceDatastore.Metadata.Columns;
                }
                else
                {
                    ddColumns.ItemsSource = targetDatastore.Metadata.Columns;
                }
                
            }
        }

        private void btnDeleteOutput_Click(object sender, RoutedEventArgs e)
        {
            this.configuration.OutputColumns.Remove((OutputColumn)((Button)sender).Tag);
        }

        private void btnAddNewOutputColumn_Click(object sender, RoutedEventArgs e)
        {
            this.configuration.OutputColumns.Add(new OutputColumn() { Column = new ColumnMetadata("Firstname"), DataStream = DataStreamSource.Left });
        }
    }
}
