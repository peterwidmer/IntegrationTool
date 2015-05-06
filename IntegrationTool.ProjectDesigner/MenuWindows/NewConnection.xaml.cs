using IntegrationTool.ProjectDesigner.Classes;
using IntegrationTool.SDK;
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

namespace IntegrationTool.ProjectDesigner.MenuWindows
{
    /// <summary>
    /// Interaction logic for NewConnection.xaml
    /// </summary>
    public partial class NewConnection : Window
    {
        public WindowResult Status { get; set; }
        public string OriginalConnectionConfiguration { get; set; }
        public ConnectionConfigurationBase ConnectionConfiguration { get; set; }
        public string ModuleName { get; set; }
        private List<ModuleDescription> modules;
        private bool newConfiguration;

        public NewConnection(List<ModuleDescription> modules)
        {
            newConfiguration = true;
            Initialize(modules);            
        }

        public NewConnection(List<ModuleDescription> modules, ConnectionConfigurationBase configuration, string originalConnectionConfiguration)
        {
            newConfiguration = false;            
            Initialize(modules);

            btnCreate.Content = "Update";

            this.ddConnectionTypes.SelectedItem = this.ddConnectionTypes.Items.Cast<ModuleDescription>()
                                                        .Where(t => t.ModuleType == configuration.ModuleDescription.ModuleType).FirstOrDefault();
            this.ddConnectionTypes.IsEnabled = false;

            this.ConnectionConfiguration = configuration;
            this.OriginalConnectionConfiguration = originalConnectionConfiguration;

            SetModule();
            ddConnectionTypes.IsEnabled = false;            
        }

        public void Initialize(List<ModuleDescription> modules)
        {
            this.modules = modules;
            InitializeComponent();
            this.ddConnectionTypes.ItemsSource = modules.
                                                    Where(t => t.Attributes.ModuleType == ModuleType.Connection).
                                                    OrderBy(t => t.Attributes.DisplayName);
        }

        private void SetModule()
        {
            this.DataContext = this.ConnectionConfiguration;
            IModule module = Activator.CreateInstance(this.ConnectionConfiguration.ModuleDescription.ModuleType) as IModule;
            this.configuration.Content = module.RenderConfigurationWindow(this.ConnectionConfiguration, null); // TODO Pass dataobject
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            bool validationSuccessful = ConfigurationValidationHelper.ValidateCurrentConfiguration(this.ConnectionConfiguration);
            if (validationSuccessful)
            {
                if (String.IsNullOrEmpty(tbConnectionName.Text))
                {
                    MessageBox.Show("Connection-Name must not be empty!");
                }
                else
                {
                    this.Status = newConfiguration ? WindowResult.Created : WindowResult.Updated;
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Status = WindowResult.Canceled;
            this.Close();
        }

        private void ddConnectionTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.newConfiguration)
            {
                ModuleDescription moduleDescription = ((ComboBox)sender).SelectedItem as ModuleDescription;
                if (moduleDescription != null)
                {
                    this.ConnectionConfiguration = Activator.CreateInstance(moduleDescription.Attributes.ConfigurationType) as ConnectionConfigurationBase;
                    this.ConnectionConfiguration.ModuleDescription = this.modules.Where(t => t.Attributes.ConfigurationType == this.ConnectionConfiguration.GetType()).First();
                    SetModule();
                }
            }
        }
    }
}
