using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections;
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

namespace IntegrationTool.Module.WriteToDynamicsCrmN2N
{
    /// <summary>
    /// Interaction logic for ConfigurationContent.xaml
    /// </summary>
    public partial class ConfigurationContent : UserControl
    {
        private IDatastore dataObject;
        private EntityMetadata entity1Metadata;
        private WriteToDynamicsCrmN2NConfiguration configuration;
        private Microsoft.Xrm.Sdk.IOrganizationService organizationService;

        public ConfigurationContent(WriteToDynamicsCrmN2NConfiguration configuration, Microsoft.Xrm.Sdk.IOrganizationService organizationService, EntityMetadata entity1Metadata, IDatastore dataObject)
        {
            InitializeComponent();

            this.entity1Metadata = entity1Metadata;
            this.organizationService = organizationService;
            this.dataObject = dataObject;
            this.configuration = configuration;

            InitializeMappingControl(entity1MappingControl, entity1Metadata, configuration.Entity1Mapping);
            InitializeEntity2Dropdown();
            if(!String.IsNullOrEmpty(configuration.Entity2Name))
            {
                foreach(var item in ddEntity2.Items)
                {
                    if(((ComboBoxItem)item).Tag.ToString() == configuration.Entity2Name)
                    {
                        ddEntity2.SelectedItem = item;
                    }
                }

            }
        }

        private void ddEntity2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            configuration.Entity2Name = ((ComboBoxItem)ddEntity2.SelectedItem).Tag.ToString();
            InitializeMapping2();
        }

        public void InitializeMapping2()
        {
            BackgroundWorker bgwEntity2Changed = new BackgroundWorker();
            bgwEntity2Changed.DoWork += bgwEntity2Changed_DoWork;
            bgwEntity2Changed.RunWorkerCompleted += bgwEntity2Changed_RunWorkerCompleted;
            bgwEntity2Changed.RunWorkerAsync();
        }

        void bgwEntity2Changed_DoWork(object sender, DoWorkEventArgs e)
        {
            EntityMetadata entityMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(organizationService, configuration.Entity2Name.Split(';')[0]);
            e.Result = new object[] { entityMetadata };
        }

        void bgwEntity2Changed_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EntityMetadata entity2Metadata = ((object[])e.Result)[0] as EntityMetadata;
            InitializeMappingControl(entity2MappingControl, entity2Metadata, configuration.Entity2Mapping);
        }

        private void InitializeMappingControl(DataMappingControl.MappingControl mappingControl, EntityMetadata entityMetadata, List<DataMappingControl.DataMapping> mapping)
        {
            List<NameDisplayName> sourceList = this.dataObject.Metadata.GetColumnsAsNameDisplayNameList();
            foreach (NameDisplayName item in sourceList)
            {
                mappingControl.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            List<NameDisplayName> targetList = Crm2013Wrapper.Crm2013Wrapper.GetAllAttributesOfEntity(entityMetadata);
            foreach (NameDisplayName item in targetList)
            {
                mappingControl.TargetList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            mappingControl.Mapping = mapping;

            mappingControl.RedrawLines();
        }

        public void InitializeEntity2Dropdown()
        {
            foreach (var m2mRelationship in entity1Metadata.ManyToManyRelationships)
            {
                if (m2mRelationship.Entity2LogicalName == entity1Metadata.LogicalName)
                    ddEntity2.Items.Add(new ComboBoxItem() { Content = m2mRelationship.Entity1LogicalName + " (" + m2mRelationship.SchemaName + ")", ToolTip = m2mRelationship.SchemaName, Tag = m2mRelationship.Entity1LogicalName + ";" + m2mRelationship.SchemaName });
                else
                    ddEntity2.Items.Add(new ComboBoxItem() { Content = m2mRelationship.Entity2LogicalName + " (" + m2mRelationship.SchemaName + ")", ToolTip = m2mRelationship.SchemaName, Tag = m2mRelationship.Entity2LogicalName + ";" + m2mRelationship.SchemaName });
            }
        }
    }
}
