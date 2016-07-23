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

namespace IntegrationTool.Module.WriteToDynamicsCrm.Logging.RecordLogList
{
    /// <summary>
    /// Interaction logic for RecordLogListControl.xaml
    /// </summary>
    public partial class RecordLogListControl : UserControl
    {
        private ObservableCollection<RecordLog> recordLogs;

        public RecordLogListControl()
        {
            InitializeComponent();
        }

        public void SetModel(ObservableCollection<RecordLog> recordLogs)
        {
            this.recordLogs = recordLogs;
            ApplyRecordFilter("all");
        }

        private void ddLogFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filter = ((ComboBoxItem)ddLogFilter.SelectedItem).Tag.ToString();
            ApplyRecordFilter(filter);
        }

        private void ApplyRecordFilter(string filter)
        {
            switch (filter)
            {
                case "successful":
                    this.DataContext = recordLogs.Where(t => String.IsNullOrEmpty(t.WriteError));
                    break;

                case "errors":
                    this.DataContext = recordLogs.Where(t => !String.IsNullOrEmpty(t.WriteError));
                    break;

                case "multiple_combined_key":
                    this.DataContext = recordLogs
                        .Where(recordLog => this.recordLogs.GroupBy(t => t.CombinedBusinessKey).Where(grp => grp.Count() > 1).Select(grp => grp.Key).Contains(recordLog.CombinedBusinessKey))
                        .OrderBy(recordLog => recordLog.CombinedBusinessKey);
                    break;

                default:
                    this.DataContext = recordLogs;
                    break;
            }
        }
    }
}
