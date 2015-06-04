using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.Module.WriteToDynamicsCrm.Execution;
using IntegrationTool.Module.WriteToDynamicsCrm.Logging;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm
{
    public partial class WriteToDynamicsCrm
    {
        private Logger logger = null;
        private IOrganizationService service = null;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Building logging database"));
            this.logger = new Logger(databaseInterface);
            this.logger.InitializeDatabase();

            reportProgress(new SimpleProgressReport("Connection to crm"));
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            service = new OrganizationService(crmConnection);

            reportProgress(new SimpleProgressReport("Loading Entitymetadata"));
            EntityMetadata entityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, this.Configuration.EntityName);
            Dictionary<string, AttributeMetadata> primaryKeyAttributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();
            foreach (string primaryKey in this.Configuration.PrimaryKeyAttributes)
            {
                AttributeMetadata attributeMetadata = entityMetaData.Attributes.Where(t => t.LogicalName == primaryKey).FirstOrDefault();
                primaryKeyAttributeMetadataDictionary.Add(primaryKey, attributeMetadata);
            }

            reportProgress(new SimpleProgressReport("Mapping attributes of records"));
            EntityMapper entityMapper = new EntityMapper(entityMetaData, dataObject.Metadata, this.Configuration.Mapping, this.Configuration.PicklistMapping);

            Entity[] entities = new Entity[dataObject.Count];
            for (int i = 0; i < dataObject.Count; i++)
            {
                object[] data = dataObject[i];

                Entity entity = new Entity(this.Configuration.EntityName);
                entityMapper.MapAttributes(entity, data);

                entities[i] = entity;
                logger.AddRecord(i);

                if(StatusHelper.MustShowProgress(i, dataObject.Count) == true)
                {
                    reportProgress(new SimpleProgressReport("Mapped " + (i + 1) + " of " + dataObject.Count + " records"));
                }
            }

            reportProgress(new SimpleProgressReport("Resolving relationship entities"));

            foreach (var relationMapping in Configuration.RelationMapping)
            {
                reportProgress(new SimpleProgressReport("Resolving relationship - load metadata for entity " + relationMapping.EntityName));
                EntityMetadata relationEntityMetadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, relationMapping.EntityName);

                reportProgress(new SimpleProgressReport("Resolving relationship - load related records"));
                JoinResolver relationResolver = new JoinResolver(service, relationEntityMetadata, relationMapping.Mapping);
                Dictionary<string, Guid[]> relatedEntities = relationResolver.BuildMassResolverIndex();

                reportProgress(new SimpleProgressReport("Resolving relationship - set relations"));

                RelationSetter relationSetter = new RelationSetter(relationEntityMetadata, relationMapping.Mapping);
                relationSetter.SetRelation(relationMapping.LogicalName, entities, dataObject, relatedEntities);
            }

            reportProgress(new SimpleProgressReport("Resolving primarykeys of records"));
            PrimaryKeyResolver primaryKeyResolver = new PrimaryKeyResolver(service, entityMetaData, primaryKeyAttributeMetadataDictionary);
            Dictionary<string, Guid[]> resolvedEntities = primaryKeyResolver.OneByOneResolver(entities);

            reportProgress(new SimpleProgressReport("Writing records to crm"));
            WriteEntity(entities, resolvedEntities, primaryKeyAttributeMetadataDictionary, reportProgress);          
        }

        private void WriteEntity(Entity[] entities, Dictionary<string, Guid[]> resolvedEntities, Dictionary<string, AttributeMetadata> primaryKeyAttributeMetadataDictionary, ReportProgressMethod reportProgress)
        {
            List<Guid> entityIds = new List<Guid>();
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                entityIds.Clear();

                // Owner may not be written, so we need to temporarily store it
                EntityReference ownerid = null;
                if (entity.Contains("ownerid"))
                {
                    ownerid = entity["ownerid"] as EntityReference;
                    entity.Attributes.Remove("ownerid");
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
                

                if (resolvedEntities[entityKey].Length == 0) // Create
                {
                    logger.SetBusinessKeyAndImportTypeForRecord(i, entityKey, ImportMode.Create);
                    CreateEntity(service, entity, entityKey, i, ownerid, statecode, statuscode, entityIds, resolvedEntities);   
                }
                else // Update
                {
                    logger.SetBusinessKeyAndImportTypeForRecord(i, entityKey, ImportMode.Update);
                    UpdateEntity(service, entity, entityKey, i, ownerid, statecode, statuscode, entityIds, resolvedEntities);   
                }

                if(StatusHelper.MustShowProgress(i, entities.Length) == true)
                {
                    reportProgress(new SimpleProgressReport("Wrote " + (i + 1) + " of " + entities.Length + " records"));
                }
            }
        }

        private void CreateEntity(IOrganizationService service, Entity entity, string entityKey, int recordNumber, EntityReference ownerid, OptionSetValue statecode, OptionSetValue statuscode, List<Guid> entityIds, Dictionary<string, Guid[]> resolvedEntities)
        {
            if (this.Configuration.ImportMode == SDK.Enums.ImportMode.Create || this.Configuration.ImportMode == SDK.Enums.ImportMode.All)
            {
                // Check if owner may be written
                if (Configuration.SetOwnerMode != SDK.Enums.ImportMode.Create &&
                    Configuration.SetOwnerMode != SDK.Enums.ImportMode.All &&
                    entity.Contains("ownerid"))
                {
                    entity.Attributes.Remove("ownerid");
                }

                try
                {
                    entity.Id = service.Create(entity);
                }
                catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                {
                    logger.SetWriteFault(recordNumber, ex.Detail.Message);
                }
                entityIds = resolvedEntities[entityKey].ToList();
                entityIds.Add(entity.Id);
                resolvedEntities[entityKey] = entityIds.ToArray();

                if (ownerid != null &&
                    (this.Configuration.SetOwnerMode == SDK.Enums.ImportMode.Create || this.Configuration.SetOwnerMode == SDK.Enums.ImportMode.All))
                {
                    Crm2013Wrapper.Crm2013Wrapper.SetOwnerOfEntity(service, entity.LogicalName, entity.Id, ownerid.LogicalName, ownerid.Id);
                }

                if (statuscode != null &&
                    (this.Configuration.SetStateMode == SDK.Enums.ImportMode.Create || this.Configuration.SetStateMode == SDK.Enums.ImportMode.All))
                {
                    Crm2013Wrapper.Crm2013Wrapper.SetStateOfEntity(service, entity.LogicalName, entity.Id, statecode, statuscode);
                }
            }
        }

        private void UpdateEntity(IOrganizationService service, Entity entity, string entityKey, int recordNumber, EntityReference ownerid, OptionSetValue statecode, OptionSetValue statuscode, List<Guid> entityIds, Dictionary<string, Guid[]> resolvedEntities)
        {
            if (this.Configuration.ImportMode == SDK.Enums.ImportMode.Update || this.Configuration.ImportMode == SDK.Enums.ImportMode.All)
            {
                if (resolvedEntities[entityKey].Length == 1 ||
                   (resolvedEntities[entityKey].Length > 1 && this.Configuration.MultipleFoundMode == SDK.Enums.MultipleFoundMode.All))
                {
                    for (int iId = 0; iId < resolvedEntities[entityKey].Length; iId++)
                    {
                        entity.Id = resolvedEntities[entityKey][iId];
                        entityIds.Add(entity.Id);

                        try
                        {
                            service.Update(entity);
                        }
                        catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
                        {
                            logger.SetWriteFault(recordNumber, ex.Detail.Message);
                        }

                        if (ownerid != null &&
                            (this.Configuration.SetOwnerMode == SDK.Enums.ImportMode.Update || this.Configuration.SetOwnerMode == SDK.Enums.ImportMode.All))
                        {
                            Crm2013Wrapper.Crm2013Wrapper.SetOwnerOfEntity(service, entity.LogicalName, entity.Id, ownerid.LogicalName, ownerid.Id);
                        }

                        if (statuscode != null &&
                            (this.Configuration.SetStateMode == SDK.Enums.ImportMode.Update || this.Configuration.SetStateMode == SDK.Enums.ImportMode.All))
                        {
                            Crm2013Wrapper.Crm2013Wrapper.SetStateOfEntity(service, entity.LogicalName, entity.Id, statecode, statuscode);
                        }
                    }
                }
            }
        }
    }
}
