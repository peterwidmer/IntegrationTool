using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.DbCommons;
using IntegrationTool.Module.WriteToMSSQL.UserControls;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace IntegrationTool.Module.WriteToMSSQL
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : UserControl, IConnectionChanged
    {
        private IDatastore dataObject;
        private WriteToMSSQLConfiguration configuration;
        private IConnection connection;
        private IDatabaseMetadataProvider metadataProvider;

        // UserControls
        private AttributeMapping attributeMapping;
        private ImportSettings existingCheck;
        private RelationMapping relationMapping;

        public ConfigurationWindow(WriteToMSSQLConfiguration configuration, IDatastore dataObject)
        {
            InitializeComponent();
            this.dataObject = dataObject;
            this.configuration = configuration;
        }

        private void ddTargetTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ddTargetTables.SelectedItem == null)
            {
                return;
            }

            DbMetadataTable selectedTable = ((ComboBoxItem)ddTargetTables.SelectedItem).Tag as DbMetadataTable;
            this.configuration.TargetTable = selectedTable.TableName;

            attributeMapping = CreateAttributeMappingWindow(selectedTable);
            existingCheck = CreateExistingCheckWindow();
            relationMapping = CreateRelationmappingWindow();

            ConfigurationContent.Content = new ConfigurationContent(attributeMapping, existingCheck, relationMapping);
        }

        public async void ConnectionChanged(IConnection connection)
        {
            this.connection = connection;
            ConfigurationContent.Content = new LoadingControl();
            ddTargetTables.IsEnabled = false;

            try
            {
                await Task.Run(() => { 
                    metadataProvider = LoadMetadata();        
                });

                ConfigurationContent.Content = null;
                CommonDbInitializationHelper.InitializeTargetTable(metadataProvider, ddTargetTables, configuration.TargetTable);   
                ddTargetTables.IsEnabled = true;
                
                
            }
            catch(Exception ex)
            {
                ConfigurationContent.Content = new MessageControl("An error occured:", ex.ToString());
            }
        }

        private IDatabaseMetadataProvider LoadMetadata()
        {
            IDatabaseMetadataProvider databaseMetadataProvider = new MsSqlMetadataProvider(this.connection);
            databaseMetadataProvider.Initialize();

            return databaseMetadataProvider;
        }

        private AttributeMapping CreateAttributeMappingWindow(DbMetadataTable selectedTable)
        {
            AttributeMapping attributeMapping = new AttributeMapping(this.configuration, this.dataObject, selectedTable);
            attributeMapping.SourceTargetMapping.MappingRowAdded += SourceTargetMapping_MappingRowAdded;
            attributeMapping.SourceTargetMapping.MappingRowDeleted += SourceTargetMapping_MappingRowDeleted;
            return attributeMapping;
        }

        void SourceTargetMapping_MappingRowAdded(DataMappingControl.DataMapping item)
        {
            existingCheck.AvailablePrimaryKeyAttributes.Add(new NameDisplayName(item.Target, item.Target));
        }

        void SourceTargetMapping_MappingRowDeleted(DataMappingControl.DataMapping item)
        {
            NameDisplayName nameDisplayName = existingCheck.AvailablePrimaryKeyAttributes.Where(t => t.Name == item.Target).FirstOrDefault();
            if (nameDisplayName != null)
            {
                existingCheck.AvailablePrimaryKeyAttributes.Remove(nameDisplayName);
            }
        }

        private ImportSettings CreateExistingCheckWindow()
        {
            ImportSettings existingCheck = new ImportSettings(this.configuration);
            foreach (var mapping in this.configuration.Mapping.OrderBy(t => t.Target))
            {
                existingCheck.AvailablePrimaryKeyAttributes.Add(new NameDisplayName(mapping.Target, mapping.Target));
            }
            return existingCheck;
        }

        private RelationMapping CreateRelationmappingWindow()
        {
            // TODO Implement
            return null;
        }
    }
}
