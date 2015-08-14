using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace IntegrationTool.ProjectDesigner.Screens
{
    /// <summary>
    /// Interaction logic for DataPreviewWindow.xaml
    /// </summary>
    public partial class DataPreviewWindow : Window
    {
        public DataPreviewWindow(IDatastore datastore)
        {
            InitializeComponent();

            this.DataPreviewGrid.DataContext = DatastoreHelper.ConvertDatastoreToTable(datastore, 10000);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
