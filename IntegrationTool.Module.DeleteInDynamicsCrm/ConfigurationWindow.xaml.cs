using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.DeleteInDynamicsCrm.UserControls;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl, IConnectionChanged
    {
        private Microsoft.Xrm.Sdk.IOrganizationService orgServiceInstance;

        private IDatastore dataObject;
        private DeleteInDynamicsCrmConfiguration configuration;

        private EntityMetadata entityMetadata;

        public ConfigurationWindow(DeleteInDynamicsCrmConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        public void ConnectionChanged(IConnection connection)
        {
            ConfigurationContent.Content = new LoadingControl();

            this.orgServiceInstance = connection.GetConnection() as Microsoft.Xrm.Sdk.IOrganizationService;

            BackgroundWorker bgwConnectionChanged = new BackgroundWorker();
            bgwConnectionChanged.DoWork += bgwConnectionChanged_DoWork;
            bgwConnectionChanged.RunWorkerCompleted += bgwConnectionChanged_RunWorkerCompleted;
            bgwConnectionChanged.RunWorkerAsync();
        }

        void bgwConnectionChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var entities = IntegrationTool.Module.Crm2013Wrapper.Crm2013Wrapper.GetAllEntities(orgServiceInstance);
                e.Result = new object[] { entities };
            }
            catch (Exception ex)
            {
                e.Result = new object[] { ex };
            }
        }

        void bgwConnectionChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<NameDisplayName> entities = ((object[])e.Result)[0] as List<NameDisplayName>;
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    ComboBoxItem comboBoxItem = new ComboBoxItem() { Content = entity.Name, ToolTip = entity.DisplayName };
                    ddEntities.Items.Add(comboBoxItem);
                    if (configuration.EntityName == entity.Name)
                    {
                        ddEntities.SelectedItem = comboBoxItem;
                    }
                }

                ddEntities.IsEnabled = true;
                ConfigurationContent.Content = null;
            }

            Exception error = ((object[])e.Result)[0] as Exception;
            if (error != null)
            {
                string message = error.Message + ((error.InnerException != null) ? "\n" + error.InnerException.Message : "");
                ConfigurationContent.Content = new MessageControl("An error occured:", message);
            }
        }

        private void ddEntities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            configuration.EntityName = ((ComboBoxItem)ddEntities.SelectedItem).Content.ToString();
            ConfigurationContent.Content = new LoadingControl();
            BackgroundWorker bgwEntityChanged = new BackgroundWorker();
            bgwEntityChanged.DoWork += bgwEntityChanged_DoWork;
            bgwEntityChanged.RunWorkerCompleted += bgwEntityChanged_RunWorkerCompleted;
            bgwEntityChanged.RunWorkerAsync(((ComboBoxItem)ddEntities.SelectedItem).Content);
        }

        void bgwEntityChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.entityMetadata = ((object[])e.Result)[0] as EntityMetadata;

            ConfigurationContent.Content = new DeletionMappingJoin(this.configuration, entityMetadata, this.dataObject);
        }

        void bgwEntityChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            string selectedEntity = e.Argument as string;
            var entityMetaData = IntegrationTool.Module.Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(orgServiceInstance, selectedEntity);
            e.Result = new object[] { entityMetaData };
        }

    }
}
