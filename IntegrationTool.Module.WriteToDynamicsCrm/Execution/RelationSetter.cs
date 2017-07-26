using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public class RelationSetter
    {
        private EntityMetadata relatedEntityMetadata;
        private List<DataMapping> relationMappings;
        private Dictionary<string, AttributeMetadata> attributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();

        public RelationSetter(EntityMetadata relatedEntityMetadata, List<DataMapping> relationMappings)
        {
            this.relatedEntityMetadata = relatedEntityMetadata;
            this.relationMappings = relationMappings;

            foreach (DataMapping relMapping in relationMappings)
            {
                AttributeMetadata attributeMetadata = relatedEntityMetadata.Attributes.Where(t => t.LogicalName == relMapping.Target).First();
                attributeMetadataDictionary.Add(attributeMetadata.LogicalName, attributeMetadata);
            }
        }

        public void SetRelation(string relationMappingLogicalName, Entity[] sourceEntities, IDatastore dataObject, Dictionary<string, Guid[]> relatedEntities)
        {
            EntityMapper entityMapper = new EntityMapper(relatedEntityMetadata.GetAttributeMetadata(), dataObject.Metadata, relationMappings, null);

            for (int i = 0; i < sourceEntities.Length; i++)
            {
                Entity relatedEntity = new Entity();
                entityMapper.MapAttributes(relatedEntity, dataObject[i]);
                string relatedEntityKey = JoinResolver.BuildExistingCheckKey(relatedEntity, attributeMetadataDictionary);
                if (relatedEntities.ContainsKey(relatedEntityKey))
                {
                    // TODO Check if attribute has already been mapped (cause multiple relations on one attribute)
                    sourceEntities[i].Attributes.Add(relationMappingLogicalName, new EntityReference(this.relatedEntityMetadata.LogicalName, relatedEntities[relatedEntityKey][0]));
                }
                else
                {
                    //throw new Exception("Could not resolve related entity with key " + relatedEntityKey);
                }

            }
        }
    }
}
