using IntegrationTool.SDK;
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

namespace IntegrationTool.ProjectDesigner.Screens.UserControls
{
    /// <summary>
    /// Interaction logic for PackageRunStatus.xaml
    /// </summary>
    public partial class PackageRunStatus : UserControl
    {
        private ObservableCollection<ProgressReport> progressReports = new ObservableCollection<ProgressReport>();

        public PackageRunStatus()
        {
            InitializeComponent();
            StatusMessageGrid.DataContext = progressReports;
        }

        public void AddStatusMessage(ProgressReport progressReport)
        {
            ProgressReport existingReport = progressReports.Where(t=> t.DesignerItem.ID == progressReport.DesignerItem.ID).FirstOrDefault();
            if(existingReport == null)
            {
                progressReports.Add(progressReport);
            }
            else
            {
                int existingIndex = progressReports.IndexOf(existingReport);
                progressReports[existingIndex] = progressReport;
            }
        }
    }
}
