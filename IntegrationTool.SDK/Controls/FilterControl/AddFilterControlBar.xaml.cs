using IntegrationTool.SDK.Data;
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
    public class FilterAddedEventArgs : EventArgs
    {
        public DataFilterType FilterType { get; set; }

        public FilterAddedEventArgs(DataFilterType dataFilterType)
        {
            this.FilterType = dataFilterType;
        }
    }

    /// <summary>
    /// Interaction logic for AddFilterControlBar.xaml
    /// </summary>
    public partial class AddFilterControlBar : UserControl
    {
        public event EventHandler<FilterAddedEventArgs> NewFilterAdded;

        public AddFilterControlBar()
        {
            InitializeComponent();
        }

        private void btnAddAndFilter_Click(object sender, RoutedEventArgs e)
        {
            RaiseFilterAddedEvent(DataFilterType.AND);
        }

        private void btnAddOrFilter_Click(object sender, RoutedEventArgs e)
        {
            RaiseFilterAddedEvent(DataFilterType.OR);
        }

        private void RaiseFilterAddedEvent(DataFilterType dataFilterType)
        {
            if(NewFilterAdded != null)
            {
                NewFilterAdded(this, new FilterAddedEventArgs(dataFilterType));
            }
        }
    }
}
