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
            this.tbInputData.Text = this.datastore[0][0].ToString();
            this.DataContext = this.configuration = configuration;
        }

        private void btnUpdateTransformationPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbTransformedXml.Text = XmlTransformation.TransformXml(tbInputData.Text, this.configuration.TransformationXslt);
                this.DataPreviewGrid.DataContext = DatastoreHelper.ConvertDatastoreToTable(this.datastore, 10000);
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
    }
}
