using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK;
using IntegrationTool.SDK;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public partial class EntityMapper
    {
        // Passed by constructor
        private EntityMetadata entityMetadata;
        private List<DataMapping> mappings;
        private List<PicklistMapping> picklistMappings;
        private DataMetadata dataMetadata;

        // Computed in constructor
        private Dictionary<string, AttributeMetadata> attributeMetadataDictionary;
        private Dictionary<string, ColumnMetadata> columnMetadataDictionary;

        public EntityMapper(EntityMetadata entityMetadata, DataMetadata dataMetadata, List<DataMapping> mappings, List<PicklistMapping> picklistMappings)
        {
            this.entityMetadata = entityMetadata;
            this.dataMetadata = dataMetadata;
            this.mappings = mappings;
            this.picklistMappings = picklistMappings;

            attributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();
            foreach (var column in mappings)
            {
                AttributeMetadata attributeMetadata = entityMetadata.Attributes.Where(t => t.LogicalName == column.Target).First();
                if (attributeMetadataDictionary.ContainsKey(attributeMetadata.LogicalName) == false)
                {
                    attributeMetadataDictionary.Add(attributeMetadata.LogicalName, attributeMetadata);
                }
            }

            columnMetadataDictionary = new Dictionary<string, ColumnMetadata>();
            foreach(var column in dataMetadata.Columns.Values)
            {
                if (columnMetadataDictionary.ContainsKey(column.ColumnName) == false)
                {
                    columnMetadataDictionary.Add(column.ColumnName, column);
                }
                else
                {
                    throw new Exception("Column " + column.ColumnName + " exists twice. Please make sure the columnnames are unique!");
                }
            }
        }
    }
}
