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

namespace IntegrationTool.Module.WriteToDynamicsCrmN2N
{
    /// <summary>
    /// Interaction logic for ConfigurationContent.xaml
    /// </summary>
    public partial class ConfigurationContent : UserControl
    {
        private IDatastore dataObject;
        private EntityMetadata entityMetadata;

        public ConfigurationContent(WriteToDynamicsCrmN2NConfiguration configuration, EntityMetadata entityMetadata, IDatastore dataObject)
        {
            InitializeComponent();

            this.entityMetadata = entityMetadata;
            this.dataObject = dataObject;
            InitializeMappingControl(entity1MappingControl, configuration.Entity1Mapping);            
        }

        private void InitializeMappingControl(DataMappingControl.MappingControl mappingControl, List<DataMappingControl.DataMapping> mapping)
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
    }
}
