using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK
{
    public partial class EntityMapperLight
    {
        // Passed by constructor
        private EntityMetadata entityMetadata;
        private List<DataMapping> mappings;
        private DataMetadata dataMetadata;

        // Computed in constructor
        private Dictionary<string, AttributeMetadata> attributeMetadataDictionary;
        private Dictionary<string, ColumnMetadata> columnMetadataDictionary;

        public EntityMapperLight(EntityMetadata entityMetadata, DataMetadata dataMetadata, List<DataMapping> mappings)
        {
            this.entityMetadata = entityMetadata;
            this.dataMetadata = dataMetadata;
            this.mappings = mappings;

            attributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();
            foreach (var column in mappings)
            {
                AttributeMetadata attributeMetadata = entityMetadata.Attributes.Where(t => t.LogicalName == column.Target).First();
                attributeMetadataDictionary.Add(attributeMetadata.LogicalName, attributeMetadata);
            }

            columnMetadataDictionary = new Dictionary<string, ColumnMetadata>();
            foreach (var column in dataMetadata.Columns.Values)
            {
                columnMetadataDictionary.Add(column.ColumnName, column);
            }
        }
    }
}
