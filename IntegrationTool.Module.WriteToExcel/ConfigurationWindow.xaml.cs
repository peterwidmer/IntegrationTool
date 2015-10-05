using IntegrationTool.Module.WriteToExcel.UserControls;
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

namespace IntegrationTool.Module.WriteToExcel
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private IDatastore dataObject;
        private WriteToExcelConfiguration configuration;

        public ConfigurationWindow(WriteToExcelConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        private void ddWriteType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.configuration.WriteType)
            {
                case WriteToExcelType.Simple:
                    this.ConfigurationContent.Content = new SimpleExcelTarget();
                    break;
            }
        }
    }
}
