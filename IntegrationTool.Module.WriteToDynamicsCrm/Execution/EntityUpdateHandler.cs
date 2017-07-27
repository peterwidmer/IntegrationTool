using IntegrationTool.Module.WriteToDynamicsCrm.SDK;
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

        public static bool OwnerMustBeSet(EntityReference sourceOwnerId, EntityReference ownerInCrm, ImportMode setOwnerMode)
        {
            if (sourceOwnerId != null && Constants.UpdateModes.Contains(setOwnerMode))
            {
                bool ownerIsEqualToCurrentOwner = ownerInCrm != null && sourceOwnerId.Id == ownerInCrm.Id;
                if (!ownerIsEqualToCurrentOwner || (setOwnerMode != ImportMode.UpdateChangedValuesOnly && setOwnerMode != ImportMode.AllChangedValuesOnly))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool StatusMustBeSet(OptionSetValue sourceStatuscode, OptionSetValue statuscodeInCrm, ImportMode setStateMode)
        {
            if (sourceStatuscode != null && Constants.UpdateModes.Contains(setStateMode))
            {
                bool statuscodeIsEqualToCurrentStatuscode = statuscodeInCrm != null && sourceStatuscode.Value == statuscodeInCrm.Value;
                if (!statuscodeIsEqualToCurrentStatuscode || (setStateMode != ImportMode.UpdateChangedValuesOnly && setStateMode != ImportMode.AllChangedValuesOnly))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
