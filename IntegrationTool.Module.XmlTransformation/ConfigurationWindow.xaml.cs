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
        private IDatastore dataObject;

        public ConfigurationWindow(XmlTransformationConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();

            this.dataObject = dataObject;
            this.tbInputData.Text = this.dataObject[0][0].ToString();
            this.DataContext = this.configuration = configuration;
        }

        private void btnUpdateTransformationPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbTransformedXml.Text = XmlTransformation.TransformXml(tbInputData.Text, this.configuration.TransformationXslt);
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
