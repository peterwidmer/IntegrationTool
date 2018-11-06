using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.Module.WriteToDynamicsCrm.Execution;
using IntegrationTool.Module.WriteToDynamicsCrm.Execution.Models;
using IntegrationTool.Module.WriteToDynamicsCrm.Logging;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK;
using Microsoft.Xrm.Sdk.Query;

namespace IntegrationTool.Module.WriteToDynamicsCrm
{
    public partial class WriteToDynamicsCrm
    {
        private Logger logger = null;
        private IOrganizationService service = null;
        private EntityUpdateHandler entityUpdateHandler = null;
        private Dictionary<string, AttributeMetadata> attributeMetadataDictionary = null;
        private EntityCollection teams = new EntityCollection();
        private EntityCollection users = new EntityCollection();

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Building logging database"));
            this.logger = new Logger(databaseInterface);
            this.logger.InitializeDatabase();

            reportProgress(new SimpleProgressReport("Connection to crm"));
            this.service = connection.GetConnection() as IOrganizationService;

            reportProgress(new SimpleProgressReport("Loading Users..."));
            this.users = service.RetrieveMultiple(new QueryExpression("systemuser") { ColumnSet = new ColumnSet(true)});

            reportProgress(new SimpleProgressReport("Loading Teams..."));
            this.teams = service.RetrieveMultiple(new QueryExpression("team") { ColumnSet = new ColumnSet(true) });

            reportProgress(new SimpleProgressReport("Loading Entitymetadata"));            
            var entityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, this.Configuration.EntityName);
            var primaryKeyAttributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();
            foreach (string primaryKey in this.Configuration.PrimaryKeyAttributes)
            {
                AttributeMetadata attributeMetadata = entityMetaData.Attributes.Where(t => t.LogicalName == primaryKey).FirstOrDefault();
                primaryKeyAttributeMetadataDictionary.Add(primaryKey, attributeMetadata);
            }

            attributeMetadataDictionary = entityMetaData.GetAttributeMetadata();

            reportProgress(new SimpleProgressReport("Initialize service-objects"));
            var entityMapper = new EntityMapper(attributeMetadataDictionary, dataObject.Metadata, Configuration.Mapping, Configuration.PicklistMapping);
            var entityAttributeComparer = new EntityAttributeComparer(attributeMetadataDictionary);
            entityUpdateHandler = new EntityUpdateHandler(entityAttributeComparer);

            reportProgress(new SimpleProgressReport("Mapping attributes of records"));
            Entity[] entities = new Entity[dataObject.Count];
            for (int i = 0; i < dataObject.Count; i++)
            {
                object[] data = dataObject[i];

                Entity entity = new Entity(this.Configuration.EntityName);
                entityMapper.MapAttributes(entity, data, this.Configuration);

                entities[i] = entity;
                logger.AddRecord(i);

                if(StatusHelper.MustShowProgress(i, dataObject.Count) == true)
                {
                    reportProgress(new SimpleProgressReport("Mapped " + (i + 1) + " of " + dataObject.Count + " records"));
                }
            }

            reportProgress(new SimpleProgressReport("Resolving relationship entities"));

            if (Configuration.LookupResolve == LookupResolve.All)
            { 
                foreach (var relationMapping in Configuration.RelationMapping)
                {
                    reportProgress(new SimpleProgressReport("Resolving relationship - load metadata for entity " + relationMapping.EntityName));
                    EntityMetadata relationEntityMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, relationMapping.EntityName);

                    reportProgress(new SimpleProgressReport("Resolving relationship - load related records"));
                    JoinResolver relationResolver = new JoinResolver(service, relationEntityMetadata, relationMapping.Mapping);
                    Dictionary<string, Guid[]> relatedEntities = relationResolver.BuildMassResolverIndex();

                    reportProgress(new SimpleProgressReport("Resolving relationship - set relations"));

                    RelationSetter relationSetter = new RelationSetter(relationEntityMetadata, relationMapping.Mapping);
                    relationSetter.SetRelation(relationMapping.LogicalName, entities, dataObject, relatedEntities, Configuration);
                }
            }

            reportProgress(new SimpleProgressReport("Resolving primarykeys of records"));
            var primaryKeyResolver = new PrimaryKeyResolver(service, entityMetaData, primaryKeyAttributeMetadataDictionary);
            var resolvedEntities = primaryKeyResolver.BatchResolver(entities, Configuration.GetAllMappedAttributes(), Configuration.BatchSizeResolving);

            reportProgress(new SimpleProgressReport("Writing records to crm"));
            WriteEntity(entities, resolvedEntities, primaryKeyAttributeMetadataDictionary, reportProgress);          
        }

        private void WriteEntity(Entity[] entities, Dictionary<string, ResolvedEntity[]> resolvedEntities, Dictionary<string, AttributeMetadata> primaryKeyAttributeMetadataDictionary, ReportProgressMethod reportProgress)
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                // Owner may not be written, so we need to temporarily store it
                EntityReference ownerid = null;
                if (entity.Contains("ownerid"))
                {
                    ownerid = entity["ownerid"] as EntityReference;
                    entity.Attributes.Remove("ownerid");

                    var c = Configuration.Mapping.SingleOrDefault(m => m.Target == "ownerid");
                    if (c != null && c.Automap)
                    {
                        switch (ownerid?.LogicalName)
                        {
                            case "team":
                                ownerid = teams.Entities.SingleOrDefault(t => t["name"].ToString() == ownerid.Name)?.ToEntityReference();
                                break;

                            case "systemuser":

                                break;
                        }
                    }
                }

                // Status may not be written, so we need to temporarily store it
                OptionSetValue statecode = null;
                OptionSetValue statuscode = null;
                if(entity.Contains("statuscode"))
                {
                    statecode = entity["statecode"] as OptionSetValue;
                    entity.Attributes.Remove("statecode");

                    statuscode = entity["statuscode"] as OptionSetValue;
                    entity.Attributes.Remove("statuscode");
                }

                string entityKey = PrimaryKeyResolver.BuildExistingCheckKey(entity, primaryKeyAttributeMetadataDictionary);

                if (!resolvedEntities.ContainsKey(entityKey)) // Create
                {
                    logger.SetBusinessKeyAndImportTypeForRecord(i, entityKey, ImportMode.Create);
                    CreateEntity(service, entity, entityKey, i, ownerid, statecode, statuscode, resolvedEntities);   
                }
                else // Update
                {
                    logger.SetBusinessKeyAndImportTypeForRecord(i, entityKey, ImportMode.Update);
                    UpdateEntity(service, entity, entityKey, i, ownerid, statecode, statuscode, resolvedEntities);   
                }

                if(StatusHelper.MustShowProgress(i, entities.Length) == true)
                {
                    reportProgress(new SimpleProgressReport("Wrote " + (i + 1) + " of " + entities.Length + " records"));
                }
            }
        }

        private void CreateEntity(IOrganizationService service, Entity entity, string entityKey, int recordNumber, EntityReference ownerid, OptionSetValue statecode, OptionSetValue statuscode, Dictionary<string, ResolvedEntity[]> resolvedEntities)
        {
            if (!Constants.CreateModes.Contains(Configuration.ImportMode)) { return; }

            // Check if owner may be written
            if (ownerid != null && Constants.CreateModes.Contains(Configuration.SetOwnerMode))
            {
                entity.Attributes.Add("ownerid", ownerid);
            }

            try
            {
                entity.Id = service.Create(entity);
            }
            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                logger.SetWriteFault(recordNumber, ex.Detail.Message);
            }

            if (statuscode != null && Constants.CreateModes.Contains(Configuration.SetStateMode))
            {
                service.SetStateOfEntity(entity.LogicalName, entity.Id, statecode, statuscode);
            }

            resolvedEntities.Add(entityKey, new ResolvedEntity[] { new ResolvedEntity(entity) });
        }

        private void UpdateEntity(IOrganizationService service, Entity entity, string entityKey, int recordNumber, EntityReference ownerid, OptionSetValue statecode, OptionSetValue statuscode, Dictionary<string, ResolvedEntity[]> resolvedEntities)
        {
            if (!Constants.UpdateModes.Contains(Configuration.ImportMode)) { return; }

            bool validForUpdate = resolvedEntities[entityKey].Length == 1 || (resolvedEntities[entityKey].Length > 1 && Configuration.MultipleFoundMode == MultipleFoundMode.All);
            if (!validForUpdate) { return; }

            for (int i = 0; i < resolvedEntities[entityKey].Length; i++)
            {
                var resolvedEntity = resolvedEntities[entityKey][i].Value;
                entityUpdateHandler.BuildEntityForUpdate(entity, resolvedEntity, Configuration.ImportMode);

                try
                {
                    if (entity.Attributes.Count > 0)
                    {
                        service.Update(entity);
                        foreach (var attribute in entity.Attributes)
                        {
                            resolvedEntity[attribute.Key] = attribute.Value;
                        }
                    }
                    else
                    {
                        // Can optionally implement - log that an empty was not updated because it was equal
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    logger.SetWriteFault(recordNumber, ex.Detail.Message);
                }

                var resolvedEntityOwnerId = resolvedEntity.Contains("ownerid") ? (EntityReference)resolvedEntity["ownerid"] : null;
                bool ownerMustBeSet = EntityUpdateHandler.OwnerMustBeSet(ownerid, resolvedEntityOwnerId, Configuration.SetOwnerMode);
                if(ownerMustBeSet)
                {
                    service.SetOwnerOfEntity(entity.LogicalName, entity.Id, ownerid.LogicalName, ownerid.Id);
                    resolvedEntity["ownerid"] = ownerid;
                }

                var resolvedEntityStatuscode = resolvedEntity.Contains("statuscode") ? (OptionSetValue)resolvedEntity["statuscode"] : null;
                bool statusMustBeSet = EntityUpdateHandler.StatusMustBeSet(statuscode, resolvedEntityStatuscode, Configuration.SetStateMode);
                if(statusMustBeSet)
                {
                    service.SetStateOfEntity(entity.LogicalName, entity.Id, statecode, statuscode);
                    resolvedEntity["statecode"] = statecode;
                    resolvedEntity["statuscode"] = statuscode;
                }
            }
        }
    }
}
