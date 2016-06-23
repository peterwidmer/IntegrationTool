using IntegrationTool.ProjectDesigner.Classes;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Configuration;
using IntegrationTool.SDK.Controls.FilterControl;
using IntegrationTool.SDK.Data.DataConditionClasses;
using IntegrationTool.SDK.Database;
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
using System.Windows.Shapes;

namespace IntegrationTool.ProjectDesigner.MenuWindows
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public WindowResult Status { get; set; }

        public ConfigurationWindowSettings ConfigurationWindowSettings { get; set; }

        public ConfigurationWindow(ConfigurationWindowSettings configurationWindowSettings)
        {
            InitializeComponent();
            this.ConfigurationWindowSettings = configurationWindowSettings;
            this.lblTitle.Content = configurationWindowSettings.title;
            this.titleImage.Content = configurationWindowSettings.titleImage;
            this.DataContext = configurationWindowSettings.configuration;
            this.ConfigurationContent.Content = configurationWindowSettings.configurationControl;
            if (configurationWindowSettings.moduleDescription.Attributes.RequiresConnection)
            {
                ddSelectConnection.DataContext = configurationWindowSettings.connections; 
                ddSelectConnection.SelectedItem = configurationWindowSettings.connections.Where(t => t.ConfigurationId == configurationWindowSettings.configuration.SelectedConnectionConfigurationId).FirstOrDefault();
            }
            else
            {
                ConnectionRowUpperSpacer.Height = new GridLength(0);
                ConnectionRow.Height = new GridLength(0);
            }

            if (configurationWindowSettings.moduleDescription.Attributes.ModuleType == ModuleType.Transformation)
            {
                TransformationConfiguration transformationConfiguration = configurationWindowSettings.configuration as TransformationConfiguration;
                TabDataFilter.Visibility = System.Windows.Visibility.Visible;

                List<AttributeImplementation> dataConditionAttributes = AssemblyHelper.LoadAllClassesImplementingSpecificAttribute<DataConditionAttribute>(System.Reflection.Assembly.GetAssembly(typeof(DataConditionAttribute)));
                filterControlPlaceHolder.Content = new FilterControl(transformationConfiguration.DataFilter, configurationWindowSettings.datastore, dataConditionAttributes);
            }
        }

        private void ddSelectConnection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConnectionConfigurationBase selectedConnection = (ConnectionConfigurationBase)ddSelectConnection.SelectedItem;
            ((StepConfigurationBase)this.DataContext).SelectedConnectionConfigurationId = selectedConnection.ConfigurationId;
            if(this.ConfigurationContent.Content as IConnectionChanged != null)
            {
                IModule connection = Activator.CreateInstance(selectedConnection.ModuleDescription.ModuleType) as IModule;
                connection.SetConfiguration(selectedConnection);
                ((IConnectionChanged)this.ConfigurationContent.Content).ConnectionChanged(connection as IConnection);
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (this.ConfigurationWindowSettings.moduleDescription.Attributes.RequiresConnection && ddSelectConnection.SelectedItem == null)
            {
                MessageBox.Show("Please select a connection before saving!", "Information");
                return;
            }

            bool validationSuccessful = ConfigurationValidationHelper.ValidateCurrentConfiguration(ConfigurationWindowSettings.configuration);            
            if (validationSuccessful)
            {
                this.Status = WindowResult.Updated;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Status = WindowResult.Canceled;
            this.Close();
        }        
    }
}
