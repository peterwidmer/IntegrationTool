using IntegrationTool.Module.WriteToDynamicsCrm.Execution.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution
{
    public class PrimaryKeyResolver
    {
        private IOrganizationService service;
        private EntityMetadata entityMetadata;
        private Dictionary<string, AttributeMetadata> primaryKeyAttributeMetadataDictionary;

        public PrimaryKeyResolver(IOrganizationService service, EntityMetadata entityMetadata, Dictionary<string, AttributeMetadata> primaryKeyAttributeMetadataDictionary)
        {
            this.service = service;
            this.entityMetadata = entityMetadata;
            this.primaryKeyAttributeMetadataDictionary = primaryKeyAttributeMetadataDictionary;
        }

        public static string BuildExistingCheckKey(Entity entity, Dictionary<string, AttributeMetadata> attributeMetadataDictionary)
        {
            string strKey = "";
            foreach (var primaryKey in attributeMetadataDictionary)
            {
                if (entity.Contains(primaryKey.Key) == false)
                    throw new Exception("Could not find all attributes for primary key");
                string val = Crm2013Wrapper.Crm2013Wrapper.GetAttributeIdentifierStringValue(entity, primaryKey.Value) + "##";
                if (val != null)
                {
                    strKey += val;
                }
                else
                {
                    strKey += "null"; // TODO This is not ok!!!
                }
            }

            return strKey;
        }

        public Dictionary<string, ResolvedEntity []> BatchResolver(Entity [] sourceEntities, string [] mappedAttributes, int batchSize)
        {
            var keyDictionary = new Dictionary<string, ResolvedEntity[]>();
            var columnSet = GetColumnSet(mappedAttributes);

            for (int i = 0; i < sourceEntities.Length; i+= batchSize)
            {
                var filterExpression = GetFilterForEntities(sourceEntities, i, batchSize);
                var resolvedEntities = Crm2013Wrapper.Crm2013Wrapper.RetrieveMultiple(service, entityMetadata.LogicalName, columnSet, filterExpression);
                AddEntitiesToDictionary(keyDictionary, resolvedEntities);
            }

            return keyDictionary;
        }

        private ColumnSet GetColumnSet(string [] mappedAttributes)
        {
            var columnSet = new ColumnSet(mappedAttributes);
            if (!columnSet.Columns.Contains(entityMetadata.PrimaryIdAttribute))
            {
                columnSet.AddColumn(entityMetadata.PrimaryIdAttribute);
            }
            return columnSet;
        }

        public FilterExpression GetFilterForEntities(Entity [] sourceEntities, int startIndex, int batchSize)
        {
            var filterExpression = new FilterExpression(LogicalOperator.Or);
            int maxIndex = (startIndex + batchSize) >= sourceEntities.Length ? sourceEntities.Length : (startIndex + batchSize);
            for (int i = startIndex; i < maxIndex; i++)
            {
                var rowConditions = BuildPrimaryKeyConditions(sourceEntities, i);
                if (rowConditions != null)
                {
                    var rowFilter = new FilterExpression(LogicalOperator.And);
                    rowFilter.Conditions.AddRange(rowConditions);
                    filterExpression.Filters.Add(rowFilter);
                }
            }

            return filterExpression;
        }

        private void AddEntitiesToDictionary(Dictionary<string, ResolvedEntity[]> keyDictionary, IEnumerable<Entity> entities)
        {
            foreach(var entity in entities)
            {
                AddEntityToDictionary(keyDictionary, entity);
            }
        }

        private void AddEntityToDictionary(Dictionary<string, ResolvedEntity[]> keyDictionary, Entity resolvedEntity)
        {
            var existingCheckKey = BuildExistingCheckKey(resolvedEntity, primaryKeyAttributeMetadataDictionary);
            var resolvedEntityStruct = new ResolvedEntity(resolvedEntity);

            if (!keyDictionary.ContainsKey(existingCheckKey))
            {
                keyDictionary.Add(existingCheckKey, new ResolvedEntity[] { resolvedEntityStruct });
            }
            else
            {
                var resolvedEntitiesOfKey = keyDictionary[existingCheckKey];
                var tempResolvedEntities = new ResolvedEntity[resolvedEntitiesOfKey.Length + 1];
                resolvedEntitiesOfKey.CopyTo(tempResolvedEntities, 0);
                tempResolvedEntities[tempResolvedEntities.Length - 1] = resolvedEntityStruct;
                keyDictionary[existingCheckKey] = tempResolvedEntities;
            }
        }

        private List<ConditionExpression> BuildPrimaryKeyConditions(Entity[] sourceEntities, int i)
        {
            var conditions = new List<ConditionExpression>();
            foreach (var primaryKey in this.primaryKeyAttributeMetadataDictionary)
            {
                if (!sourceEntities[i].Contains(primaryKey.Key)) { return null; }

                var conditionValue = sourceEntities[i][primaryKey.Key];
                if (conditionValue is EntityReference)
                {
                    conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.Equal, ((EntityReference)conditionValue).Id));
                }
                else if (conditionValue is Guid)
                {
                    conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.Equal, ((Guid)conditionValue)));
                }
                else if(conditionValue is OptionSetValue)
                {
                    conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.Equal, ((EntityReference)conditionValue).Id));
                }
                else
                {
                    conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.Equal, conditionValue));
                }
            }
            return conditions;
        }


    }
}
