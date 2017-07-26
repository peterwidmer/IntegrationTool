using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public class EntityUpdateHandler
    {
        private EntityAttributeComparer entityAttributeComparer;

        public EntityUpdateHandler(EntityAttributeComparer entityAttributeComparer)
        {
            this.entityAttributeComparer = entityAttributeComparer;
        }

        public void BuildEntityForUpdate(Entity sourceEntity, Entity entityInCrm, ImportMode importMode)
        {
            sourceEntity.Id = entityInCrm.Id;
            if(importMode == ImportMode.UpdateChangedValuesOnly || importMode == ImportMode.AllChangedValuesOnly)
            {
                RemoveAttributesIfEqualToCrmEntity(sourceEntity, entityInCrm);
            }
        }

        private void RemoveAttributesIfEqualToCrmEntity(Entity sourceEntity, Entity entityInCrm)
        {
            var attributesToRemove = new List<string>();

            foreach (var sourceAttribute in sourceEntity.Attributes)
            {
                if (sourceAttribute.Value == null && !entityInCrm.Contains(sourceAttribute.Key))
                {
                    attributesToRemove.Add(sourceAttribute.Key);
                }
                else if(sourceAttribute.Value == null && entityInCrm.Contains(sourceAttribute.Key))
                {
                    continue;
                }
                else if(sourceAttribute.Value != null && !entityInCrm.Contains(sourceAttribute.Key))
                {
                    continue;
                }
                else
                {
                    bool attributesAreEqual = entityAttributeComparer.AttributesAreEqual(sourceAttribute.Key, sourceAttribute.Value, entityInCrm[sourceAttribute.Key]);
                    if(attributesAreEqual)
                    {
                        attributesToRemove.Add(sourceAttribute.Key);
                    }
                }
            }

            foreach (var attributeToRemove in attributesToRemove)
            {
                sourceEntity.Attributes.Remove(attributeToRemove);
            }
        }
    }
}
