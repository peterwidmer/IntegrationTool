using IntegrationTool.Module.WriteToDynamicsCrm.UserControls.Mapping;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using Microsoft.Xrm.Sdk.Metadata;
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
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;

namespace IntegrationTool.Module.WriteToDynamicsCrm.UserControls
{
    /// <summary>
    /// Interaction logic for AttributeMapping.xaml
    /// </summary>
    public partial class AttributeMapping : UserControl
    {
        private EntityMetadata entityMetadata;
        private IDatastore dataObject;
        private WriteToDynamicsCrmConfiguration configuration;

        public AttributeMapping(WriteToDynamicsCrmConfiguration configuration, EntityMetadata entityMetadata, IDatastore dataObject)
        {
            InitializeComponent();
            this.configuration = configuration;
            this.entityMetadata = entityMetadata;
            this.dataObject = dataObject;
            InitializeMappingControl(configuration.Mapping);

            this.SourceTargetMapping.MappingRowAdded += SourceTargetMapping_MappingRowAdded;
            this.SourceTargetMapping.MappingRowDeleted += SourceTargetMapping_MappingRowDeleted;
            this.SourceTargetMapping.ListBoxTargetKlick += SourceTargetMapping_ListBoxTargetKlick;
        }

        void SourceTargetMapping_ListBoxTargetKlick(object sender, EventArgs e)
        {
            ListViewItem clickedItem = sender as ListViewItem;
            IntegrationTool.DataMappingControl.DataMapping item = this.SourceTargetMapping.Mapping.Where(t => t.Target == clickedItem.Content.ToString()).FirstOrDefault();
            if(item != null)
            {
                ShowMappingDetails(item);
            }
            else
            {
                attributeDependentSettings.Content = null;
            }
        }

        void SourceTargetMapping_MappingRowAdded(DataMappingControl.DataMapping item)
        {

            ShowMappingDetails(item);
        }

        private void ShowMappingDetails(DataMappingControl.DataMapping item)
        {
            AttributeMetadata attributeMetadata = entityMetadata.Attributes.Where(t => t.LogicalName == item.Target).First();
            switch (attributeMetadata.AttributeType.Value)
            {
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist:
                    var picklistMappingEntry = this.configuration.PicklistMapping.Where(t => t.LogicalName == item.Target).FirstOrDefault();
                    if (picklistMappingEntry == null)
                    {
                        picklistMappingEntry = new SDK.PicklistMapping() { LogicalName = item.Target };

                        if (item.Automap)
                        {
                            picklistMappingEntry.MappingType = PicklistMappingType.Automatic;
                        }

                        this.configuration.PicklistMapping.Add(picklistMappingEntry);
                    }

                    List<NameDisplayName> sourceList = this.dataObject.GetDistinctValuesOfColumn(item.Source);
                    foreach (var alreadyMappedValue in picklistMappingEntry.Mapping)
                    {
                        if (!sourceList.Any(t => t.Name == alreadyMappedValue.Source))
                        {
                            sourceList.Add(new NameDisplayName(alreadyMappedValue.Source, alreadyMappedValue.Source));
                        }
                    }
                    List<NameDisplayName> targetList = Crm2013Wrapper.Crm2013Wrapper.GetPicklistValuesOfPicklistAttributeMetadata(attributeMetadata);

                    PicklistMapping picklistMapping = new PicklistMapping(picklistMappingEntry, sourceList, targetList);
                    attributeDependentSettings.Content = picklistMapping;
                    break;

                case AttributeTypeCode.DateTime:
                    attributeDependentSettings.Content = new DateTimeMapping(item);
                    break;

                default:
                    attributeDependentSettings.Content = new DefaultMapping(item);
                        
                    break;
            }
        }

        

        void SourceTargetMapping_MappingRowDeleted(DataMappingControl.DataMapping item)
        {
            var picklistMappingEntry = this.configuration.PicklistMapping.Where(t=>t.LogicalName == item.Target).FirstOrDefault();
            if(picklistMappingEntry != null)
            {
                this.configuration.PicklistMapping.Remove(picklistMappingEntry);
            }
        }

        private void InitializeMappingControl(List<DataMappingControl.DataMapping> mapping)
        {
            List<NameDisplayName> sourceList = this.dataObject.Metadata.GetColumnsAsNameDisplayNameList();
            foreach(NameDisplayName item in sourceList)
            {
                SourceTargetMapping.SourceList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            List<NameDisplayName> targetList = Crm2013Wrapper.Crm2013Wrapper.GetAllAttributesOfEntity(entityMetadata);
            foreach (NameDisplayName item in targetList)
            {
                SourceTargetMapping.TargetList.Add(new ListViewItem() { Content = item.Name, ToolTip = item.DisplayName });
            }

            SourceTargetMapping.Mapping = mapping;
        }
    }
}
