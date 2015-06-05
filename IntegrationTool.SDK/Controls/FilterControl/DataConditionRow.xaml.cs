using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Data.DataConditionClasses;
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

namespace IntegrationTool.SDK.Controls.FilterControl
{
    /// <summary>
    /// Interaction logic for DataConditionRow.xaml
    /// </summary>
    public partial class DataConditionRow : UserControl
    {
        private IDatastore datastore;
        private List<AttributeImplementation> dataConditionAttributes;

        public DataConditionRow(DataCondition dataCondition, IDatastore datastore, List<AttributeImplementation> dataConditionAttributes)
        {
            InitializeComponent();
            this.datastore = datastore;
            this.dataConditionAttributes = dataConditionAttributes;
            
            InitializeColumns();

            
            this.DataContext = dataCondition;
        }

        private void InitializeColumns()
        {
            foreach(var column in datastore.Metadata.Columns.Values)
            {
                ddColumn.Items.Add(new ComboBoxItem() { Content = column.ColumnName, Tag = column.ColumnName });
            }
        }

        private void ddTreatAsType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ddDataConditionType.Items.Clear();
            foreach (var attributeImplementation in dataConditionAttributes.OrderBy(t => ((DataConditionAttribute)t.Attribute).ConditionName))
            {
                DataConditionAttribute attribute = attributeImplementation.Attribute as DataConditionAttribute;
                if(attribute.ConditionType.ToString() == ddTreatAsType.SelectedItem.ToString())
                {
                    ddDataConditionType.Items.Add(new ComboBoxItem() { Content = attribute.ConditionName, Tag = attribute.ConditionName });
                }
            }

            ddDataConditionType.SelectedIndex = 0;
        }

        private void ddDataConditionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tbValue.Visibility = System.Windows.Visibility.Visible;
            if(ddDataConditionType.SelectedItem == null)
            {
                tbValue.Text = null;
                return;
            }

            foreach(var attributeImplementation in dataConditionAttributes)
            {
                DataConditionAttribute attribute = attributeImplementation.Attribute as DataConditionAttribute;
                if(attribute.ConditionType.ToString() == ddTreatAsType.SelectedItem.ToString() &&
                    ((ComboBoxItem)ddDataConditionType.SelectedItem).Tag.ToString() == attribute.ConditionName)
                {
                    tbValue.Visibility = attribute.ValueVisibility;
                }
            }
        }
    }
}
