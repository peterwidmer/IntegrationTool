using IntegrationTool.Module.ConnectToExcel.UserControls;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace IntegrationTool.Module.ConnectToExcel
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private ConnectToExcelConfiguration configuration;

        public ConfigurationWindow(ConnectToExcelConfiguration configuration)
        {
            InitializeComponent();
            this.DataContext = this.configuration = configuration;   
        }

        private void ddConnectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(this.configuration.ConnectionType)
            {
                case ExcelConnectionType.ExistingFileAndSheet:
                    ConfigurationContent.Content = new ExistingFileAndSheetControl(this.configuration);
                    break;

                case ExcelConnectionType.NewFileAndSheet:
                    ConfigurationContent.Content = new NewFileAndSheetControl(this.configuration);
                    break;
            }
        }
    }
}
