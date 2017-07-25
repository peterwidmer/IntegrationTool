using IntegrationTool.Module.WriteToDynamicsCrm.Execution.Models;
using IntegrationTool.SDK;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;
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

        public void MassResolver()
        {
            Dictionary<string, Guid[]> existingEntityIDs = BuildMassResolverIndex();
        }

        public Dictionary<string, Guid[]> BuildMassResolverIndex()
        {

            ColumnSet columnSet = new ColumnSet();
            foreach(var primaryKey in primaryKeyAttributeMetadataDictionary)
            {
                columnSet.AddColumn(primaryKey.Key);
            }
            if (columnSet.Columns.Contains(entityMetadata.PrimaryIdAttribute) == false)
            {
                columnSet.AddColumn(entityMetadata.PrimaryIdAttribute);
            }
            List<ConditionExpression> conditions = new List<ConditionExpression>();
            foreach (var primaryKey in primaryKeyAttributeMetadataDictionary)
            {
                conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.NotNull));
            }

            DataCollection<Entity> existingEntities = Crm2013Wrapper.Crm2013Wrapper.RetrieveMultiple(service, entityMetadata.LogicalName, columnSet, conditions);

            Dictionary<string, Guid[]> keyDictionary = BuildExistingCheckDictionary(existingEntities, primaryKeyAttributeMetadataDictionary);

            return keyDictionary;
        }

        public Dictionary<string, Guid[]> BuildExistingCheckDictionary(DataCollection<Entity> entities, Dictionary<string, AttributeMetadata> attributeMetadataDictionary)
        {
            Dictionary<string, Guid[]> keyDictionary = new Dictionary<string, Guid[]>();
            foreach (Entity entity in entities)
            {

                string strKey = BuildExistingCheckKey(entity, attributeMetadataDictionary);
                if (keyDictionary.ContainsKey(strKey) == false)
                {
                    keyDictionary.Add(strKey, new Guid[] { entity.Id });
                }
                else
                {
                    List<Guid> Ids = keyDictionary[strKey].ToList();
                    Ids.Add(entity.Id);
                    keyDictionary[strKey] = Ids.ToArray();
                }
            }

            return keyDictionary;
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

        public Dictionary<string, ResolvedEntity []> OneByOneResolver(Entity [] sourceEntities, string [] mappedAttributes)
        {
            var columnSet = new ColumnSet(mappedAttributes);
            if(!columnSet.Columns.Contains(entityMetadata.PrimaryIdAttribute))
            {
                columnSet.AddColumn(entityMetadata.PrimaryIdAttribute);
            }

            var keyDictionary = new Dictionary<string, ResolvedEntity[]>();
            for (int i = 0; i < sourceEntities.Length; i++)
            {
                var conditions = BuildPrimaryKeyConditions(sourceEntities, i);
                var existingCheckKey = BuildExistingCheckKey(sourceEntities[i], primaryKeyAttributeMetadataDictionary);

                var entities = Crm2013Wrapper.Crm2013Wrapper.RetrieveMultiple(service, entityMetadata.LogicalName, columnSet, conditions);
                AddEntitiesToDictionary(keyDictionary, existingCheckKey, entities);
            }

            return keyDictionary;
        }

        private void AddEntitiesToDictionary(Dictionary<string, ResolvedEntity[]> keyDictionary, string existingCheckKey, DataCollection<Entity> entities)
        {
            if (!keyDictionary.ContainsKey(existingCheckKey))
            {
                keyDictionary.Add(existingCheckKey, new ResolvedEntity[entities.Count]);
                for (int iRetrievedEntity = 0; iRetrievedEntity < entities.Count; iRetrievedEntity++)
                {
                    var serializedEntity = JsonConvert.SerializeObject(entities[iRetrievedEntity]);
                    var resolvedEntity = new ResolvedEntity(entities[iRetrievedEntity].Id, serializedEntity);
                    keyDictionary[existingCheckKey][iRetrievedEntity] = resolvedEntity;
                }
            }
        }

        private List<ConditionExpression> BuildPrimaryKeyConditions(Entity[] sourceEntities, int i)
        {
            var conditions = new List<ConditionExpression>();
            foreach (var primaryKey in this.primaryKeyAttributeMetadataDictionary)
            {
                conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.Equal, sourceEntities[i][primaryKey.Key]));
            }
            return conditions;
        }


    }
}
