using IntegrationTool.Module.ConcatenateColumns.SDK;
using IntegrationTool.SDK;
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

namespace IntegrationTool.Module.ConcatenateColumns
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private ConcatenateColumnsConfiguration configuration;
        private IDatastore dataObject;

        private List<ComboBoxItem> Columns = new List<ComboBoxItem>();

        public ConfigurationWindow(ConcatenateColumnsConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.DataContext = this.configuration = configuration;
            this.dataObject = dataObject;
        }

        private void AddNewConcatenation_Click(object sender, RoutedEventArgs e)
        {
            this.configuration.ColumnConcatenations.Add(new ColumnConcatenation());
        }

        private void DataTemplateLoaded_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = ((Label)sender).Parent as Grid;
            ComboBox ddLeftColumn = WPFHelper.FindVisualChildren<ComboBox>(grid).Where(t => t.Name == "ddLeftColumn").First();
            ComboBox ddRightColumn = WPFHelper.FindVisualChildren<ComboBox>(grid).Where(t => t.Name == "ddRightColumn").First();

            foreach (var column in dataObject.Metadata.Columns)
            {
                ddLeftColumn.Items.Add(new ComboBoxItem() { Content = column.ColumnName, Tag = column.ColumnName });
                ddRightColumn.Items.Add(new ComboBoxItem() { Content = column.ColumnName, Tag = column.ColumnName });
            }
        }
    }
}
