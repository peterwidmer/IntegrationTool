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
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class FilterControl : UserControl
    {
        private DataFilter dataFilter;
        private IDatastore datastore;
        private List<AttributeImplementation> dataConditionAttributes;

        public FilterControl(DataFilter dataFilter, IDatastore datastore, List<AttributeImplementation> dataConditionAttributes)
        {
            InitializeComponent();

            this.dataFilter = dataFilter;
            this.datastore = datastore;
            this.dataConditionAttributes = dataConditionAttributes;

            if (dataFilter.FilterType == null)
            {
                this.addFilterControlBar.NewFilterAdded += addFilterControlBar_NewFilterAdded;
            }
            else
            {
                this.addFilterControlBar.Visibility = System.Windows.Visibility.Hidden;
                this.FilterPanel.Children.Add(new Filter(dataFilter, this.datastore, this.dataConditionAttributes));
            }
        }

        void addFilterControlBar_NewFilterAdded(object sender, FilterAddedEventArgs e)
        {
            this.addFilterControlBar.Visibility = System.Windows.Visibility.Hidden;

            this.dataFilter.FilterType = e.FilterType;
            this.FilterPanel.Children.Add(new Filter(dataFilter, this.datastore, this.dataConditionAttributes));
        }


    }
}
