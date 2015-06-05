using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrationTool.Module.StringTranformation
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private List<StringTransformationAttribute> transformationAttributes;
        private StringTransformationConfiguration configuration;
        private IDatastore dataObject;

        public ConfigurationWindow(StringTransformationConfiguration configuration, List<StringTransformationAttribute> transformationAttributes, IDatastore dataObject)
        {
            InitializeComponent();

            this.transformationAttributes = transformationAttributes;
            this.dataObject = dataObject;
            this.DataContext = this.configuration = configuration;
        }

        private void ddStringTransformationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox ddStringTransformationType = sender as ComboBox;
            if(ddStringTransformationType.SelectedItem != null)
            {
                ComboBoxItem transformationItem = ddStringTransformationType.SelectedItem as ComboBoxItem;
                var transformation = transformationAttributes.Where(t => t.TransformationType.ToString() == transformationItem.Tag.ToString()).FirstOrDefault();

                SetParam1(ddStringTransformationType, transformation.Param1Visibility, transformation.Param1Label);
                SetParam2(ddStringTransformationType, transformation.Param2Visibility, transformation.Param2Label);
            }
        }

        private void SetParam1(ComboBox ddStringTransformationType, System.Windows.Visibility visibility, string label)
        {
            Label lblParam1 = WPFHelper.FindVisualChildren<Label>(ddStringTransformationType.Parent).Where(t => t.Name == "lblParam1").First();
            lblParam1.Visibility = visibility;
            lblParam1.Content = label;

            TextBox tbParam1 = WPFHelper.FindVisualChildren<TextBox>(ddStringTransformationType.Parent).Where(t => t.Name == "tbParam1").First();
            tbParam1.Visibility = visibility;
        }

        private void SetParam2(ComboBox ddStringTransformationType, System.Windows.Visibility visibility, string label)
        {
            Label lblParam2 = WPFHelper.FindVisualChildren<Label>(ddStringTransformationType.Parent).Where(t => t.Name == "lblParam2").First();
            lblParam2.Visibility = visibility;
            lblParam2.Content = label;

            TextBox tbParam2 = WPFHelper.FindVisualChildren<TextBox>(ddStringTransformationType.Parent).Where(t => t.Name == "tbParam2").First();
            tbParam2.Visibility = visibility;
        }

        private void AddNewStringTransformation_Click(object sender, RoutedEventArgs e)
        {
            this.configuration.Transformations.Add(new StringTransformationParameter());
        }

        private void DataTemplateLoaded_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = ((Label)sender).Parent as Grid;
            ComboBox ddColumn = WPFHelper.FindVisualChildren<ComboBox>(grid).Where(t => t.Name == "ddColumn").First();
            foreach(var column in dataObject.Metadata.Columns.Values)
            {
                ddColumn.Items.Add(new ComboBoxItem() { Content = column.ColumnName, Tag = column.ColumnName });
            }
        }


    }
}
