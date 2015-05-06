using IntegrationTool.SDK.Data;
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
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filter : UserControl
    {
        private DataFilter dataFilter;
        private IDatastore datastore;
        private List<AttributeImplementation> dataConditionAttributes;

        public Filter(DataFilter dataFilter, IDatastore datastore, List<AttributeImplementation> dataConditionAttributes)
        {
            InitializeComponent();
            this.datastore = datastore;
            this.DataContext = this.dataFilter = dataFilter;
            this.dataConditionAttributes = dataConditionAttributes;

            InitializeFilter();
        }

        private void InitializeFilter()
        {
            FilterTitle.Content += (dataFilter.FilterType == DataFilterType.AND) ? " AND" : " OR";
            
            foreach (DataCondition dataCondition in this.dataFilter.DataConditions)
            {
                AddDataCondition(dataCondition);
            }

            foreach(DataFilter filter in this.dataFilter.Filters)
            {
                AddDataFilter(filter);
            }
        }

        private void btnAddCondition_Click(object sender, RoutedEventArgs e)
        {
            DataCondition dataCondition = new DataCondition();
            this.dataFilter.DataConditions.Add(dataCondition);
            AddDataCondition(dataCondition);
        }

        private void AddDataCondition(DataCondition dataCondition)
        {
            conditionsPanel.Children.Add(new DataConditionRow(dataCondition, this.datastore, this.dataConditionAttributes));
        }

        private void AddDataFilter(DataFilter filter)
        {
            conditionsPanel.Children.Add(new Filter(filter, this.datastore, this.dataConditionAttributes));
        }

        private void AddFilterControlBar_NewFilterAdded(object sender, FilterAddedEventArgs e)
        {
            DataFilter newDataFilter = new DataFilter();
            newDataFilter.FilterType = e.FilterType;
            this.dataFilter.Filters.Add(newDataFilter);
            this.conditionsPanel.Children.Add(new Filter(newDataFilter, this.datastore, this.dataConditionAttributes));
        }
    }
}
