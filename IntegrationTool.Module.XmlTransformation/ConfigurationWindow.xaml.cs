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
using System.Xml;

namespace IntegrationTool.Module.XmlTransformation
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl
    {
        private XmlTransformationConfiguration configuration;
        private IDatastore datastore;

        public ConfigurationWindow(XmlTransformationConfiguration configuration, IDatastore datastore)
        {
            InitializeComponent();

            this.datastore = datastore;
            this.InputXmlColumn.ItemsSource = datastore.Metadata.GetColumnsAsNameDisplayNameList();
            this.DataContext = this.configuration = configuration;
        }

        private void btnUpdateTransformationPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int columnIndex = datastore.Metadata.Columns[this.configuration.InputXmlColumn].ColumnIndex;
                string originalDataStore = (string)datastore[0][columnIndex];
                tbTransformedXml.Text = XmlTransformation.TransformXml(tbInputData.Text, this.configuration.TransformationXslt);
                var transformationLog = XmlTransformation.TransformToDatastore(datastore, this.configuration.TransformationXslt, this.configuration.InputXmlColumn, true);
                this.DataPreviewGrid.DataContext = DatastoreHelper.ConvertDatastoreToTable(this.datastore, 10000, new string[] { this.configuration.InputXmlColumn }, transformationLog.RowNumbersToHide.ToArray());
                foreach(var newColumn in transformationLog.NewColumns)
                {
                    datastore.RemoveColumn(newColumn);
                }
                datastore[0][columnIndex] = originalDataStore;
            }
            catch(Exception ex)
            {
                string errorMessage = "An error occured while transforming:\n\n" + ex.Message;
                if(ex.InnerException != null)
                {
                    errorMessage += ex.InnerException.Message;
                }
                MessageBox.Show(errorMessage);
            }
        }

        private void InputXmlColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InputXmlColumn.SelectedItem != null)
            {
                int columnIndex = this.datastore.Metadata.Columns[this.configuration.InputXmlColumn].ColumnIndex;
                this.tbInputData.Text = this.datastore[0][columnIndex].ToString();
            }

            this.tbTransformedXml.Text = string.Empty;
            this.DataPreviewGrid.DataContext = null;
        }
    }
}
