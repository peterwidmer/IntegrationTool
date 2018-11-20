using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToDynamicsCrm.UserControls;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
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

namespace IntegrationTool.Module.WriteToDynamicsCrm
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl, IConnectionChanged
    {
        private Microsoft.Xrm.Sdk.IOrganizationService orgServiceInstance;
        private EntityMetadata entityMetadata;
        private IDatastore dataObject;
        private WriteToDynamicsCrmConfiguration configuration;

        // UserControls
        private AttributeMapping attributeMapping;
        private ImportSettings existingCheck;
        private RelationMapping relationMapping;

        public ConfigurationWindow(WriteToDynamicsCrmConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        public void ConnectionChanged(IConnection connection)
        {
            ConfigurationContent.Content = new LoadingControl();
            ddEntities.IsEnabled = false;

            this.orgServiceInstance = connection.GetConnection() as Microsoft.Xrm.Sdk.IOrganizationService;

            BackgroundWorker bgwConnectionChanged = new BackgroundWorker();
            bgwConnectionChanged.DoWork += bgw_DoWork;
            bgwConnectionChanged.RunWorkerCompleted += bgwConnectionChanged_RunWorkerCompleted;
            bgwConnectionChanged.RunWorkerAsync();         
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
            if(error != null)
            {
                string message = error.Message + ((error.InnerException != null) ? "\n" + error.InnerException.Message : "");
                ConfigurationContent.Content = new MessageControl("An error occured:", message);
            }
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var entities = IntegrationTool.Module.Crm2013Wrapper.Crm2013Wrapper.GetAllEntities(orgServiceInstance);
                e.Result = new object[] { entities };
            }
            catch(Exception ex)
            {
                e.Result = new object[] { ex };
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

            attributeMapping = CreateAttributeMappingWindow(this.entityMetadata);
            existingCheck = CreateImportSettingsControl();
            relationMapping = CreateRelationmappingWindow(this.entityMetadata);

            ConfigurationContent.Content = new ConfigurationContent(attributeMapping, existingCheck, relationMapping);         
        }

        void bgwEntityChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            string selectedEntity = e.Argument as string;
            var entityMetaData = IntegrationTool.Module.Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(orgServiceInstance, selectedEntity);
            e.Result = new object[] { entityMetaData };
        }

        private AttributeMapping CreateAttributeMappingWindow(EntityMetadata entityMetadata)
        {   
            AttributeMapping attributeMapping = new AttributeMapping(this.configuration, entityMetadata, this.dataObject);
            attributeMapping.SourceTargetMapping.MappingRowAdded += SourceTargetMapping_MappingRowAdded;
            attributeMapping.SourceTargetMapping.MappingRowDeleted += SourceTargetMapping_MappingRowDeleted;
            return attributeMapping;
        }       

        void SourceTargetMapping_MappingRowAdded(DataMappingControl.DataMapping item)
        {
            existingCheck.AvailablePrimaryKeyAttributes.Add(new NameDisplayName(item.Target, item.Target));
            existingCheck.AvailablePrimaryKeyAttributes.OrderBy(i => i.DisplayName);
        }

        void SourceTargetMapping_MappingRowDeleted(DataMappingControl.DataMapping item)
        {
            NameDisplayName nameDisplayName = existingCheck.AvailablePrimaryKeyAttributes.Where(t => t.Name == item.Target).FirstOrDefault();
            if(nameDisplayName != null)
            {
                existingCheck.AvailablePrimaryKeyAttributes.Remove(nameDisplayName);
            }
        }

        private ImportSettings CreateImportSettingsControl()
        {
            ImportSettings existingCheck = new ImportSettings(this.configuration);
            foreach (var mapping in GetPrimaryKeyAttributes())
            {
                existingCheck.AvailablePrimaryKeyAttributes.Add(mapping);
            }
            return existingCheck;
        }

        private IEnumerable<NameDisplayName> GetPrimaryKeyAttributes()
        {
            var resultList = new List<NameDisplayName>();
            resultList.AddRange(configuration.Mapping.Select(mapping => new NameDisplayName(mapping.Target, mapping.Target)));
            resultList.AddRange(configuration.RelationMapping.Select(mapping => new NameDisplayName(mapping.LogicalName, mapping.LogicalName)));

            return resultList.OrderBy(t => t.DisplayName);
        }

        private RelationMapping CreateRelationmappingWindow(EntityMetadata entityMetadata)
        {
            RelationMapping relationMapping = new RelationMapping(orgServiceInstance, this.configuration, this.dataObject, entityMetadata);
            return relationMapping;
        }
    }
}
