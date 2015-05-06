using IntegrationTool.SDK;
using Microsoft.Xrm.Client;
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

        public Dictionary<string, Guid []> OneByOneResolver(Entity [] sourceEntities)
        {
            Dictionary<string, Guid[]> keyDictionary = new Dictionary<string, Guid[]>();
            for (int i = 0; i < sourceEntities.Length; i++)
            {
                List<ConditionExpression> conditions = new List<ConditionExpression>();
                foreach (var primaryKey in this.primaryKeyAttributeMetadataDictionary)
                {
                    conditions.Add(new ConditionExpression(primaryKey.Key, ConditionOperator.Equal, sourceEntities[i][primaryKey.Key]));
                }

                string key = BuildExistingCheckKey(sourceEntities[i], primaryKeyAttributeMetadataDictionary);
                DataCollection<Entity> entities = Crm2013Wrapper.Crm2013Wrapper.RetrieveMultiple(service, this.entityMetadata.LogicalName, new ColumnSet(new string[] { this.entityMetadata.PrimaryIdAttribute }), conditions);
                keyDictionary.Add(key, new Guid [entities.Count]);
                for (int iRetrievedEntity = 0; iRetrievedEntity < entities.Count; iRetrievedEntity++)
                {
                    keyDictionary[key][iRetrievedEntity] = entities[iRetrievedEntity].Id;
                }
            }

            return keyDictionary;
        }


    }
}
