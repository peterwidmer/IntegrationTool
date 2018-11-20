using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.Module.WriteToDynamicsCrm.Execution;
using IntegrationTool.Module.WriteToDynamicsCrm.Execution.Models;
using IntegrationTool.Module.WriteToDynamicsCrm.Logging;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;

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
        private EntityCollection currency = new EntityCollection();
        private EntityCollection businessunit = new EntityCollection();

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Building logging database"));
            this.logger = new Logger(databaseInterface);
            this.logger.InitializeDatabase();

            reportProgress(new SimpleProgressReport("Connection to crm"));
            this.service = connection.GetConnection() as IOrganizationService;

            reportProgress(new SimpleProgressReport("Loading Users..."));
            this.users = service.RetrieveMultiple(new QueryExpression("systemuser") { ColumnSet = new ColumnSet(true) });

            reportProgress(new SimpleProgressReport("Loading Teams..."));
            this.teams = service.RetrieveMultiple(new QueryExpression("team") { ColumnSet = new ColumnSet(true) });

            reportProgress(new SimpleProgressReport("Loading currency..."));
            this.currency = service.RetrieveMultiple(new QueryExpression("systemuser") { ColumnSet = new ColumnSet(true) });

            reportProgress(new SimpleProgressReport("Loading BusinesUnit..."));
            this.businessunit = service.RetrieveMultiple(new QueryExpression("team") { ColumnSet = new ColumnSet(true) });

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

                if (StatusHelper.MustShowProgress(i, dataObject.Count) == true)
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
                try
                {



                Entity entity = entities[i];
                ResolveUser(entity);

                var partyLists = entity.Attributes.Where(t => t.Value is EntityCollection).ToList();
                foreach (var partyList in partyLists)
                {
                    var c = Configuration.Mapping.SingleOrDefault(m => m.Target == partyList.Key);
                    if ((c != null && c.Automap))
                    {
                        foreach (var party in ((EntityCollection)partyList.Value).Entities)
                        {
                            ResolveParty(party);
                        }
                    }
                }

                ResolveAutoMapRelation(entity, currency, "transactioncurrency");
                ResolveAutoMapRelation(entity, businessunit, "businessunit");


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
                if (entity.Contains("statuscode"))
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

                if (StatusHelper.MustShowProgress(i, entities.Length) == true)
                {
                    reportProgress(new SimpleProgressReport("Wrote " + (i + 1) + " of " + entities.Length + " records"));
                }
                }
                catch (Exception ex)
                {
                    logger.SetWriteFault(i, ex.Message);
                }
            }
        }

        private void ResolveAutoMapRelation(Entity entity, EntityCollection entityCollection, string transactioncurrency)
        {
            var currencyattribute = entity.Attributes.Where(t =>
                t.Value is EntityReference && ((EntityReference) t.Value).LogicalName == transactioncurrency).ToList();
            foreach (var currentcurrency in currencyattribute)
            {
                var cd = Configuration.Mapping.SingleOrDefault(m => m.Target == currentcurrency.Key);
                if ((cd != null && cd.Automap))
                {
                    EntityReference newcurrency = entityCollection.Entities
                        .SingleOrDefault(c => c.Contains("name") && c["name"].ToString() == ((EntityReference) currentcurrency.Value).Name)
                        ?.ToEntityReference();

                    if (newcurrency == null)
                    {
                        entity.Attributes.Remove(currentcurrency.Key);
                    }
                    else
                    {
                        entity[currentcurrency.Key] = newcurrency;
                    }
                }
            }
        }

        private void ResolveParty(Entity entity)
        {
            var userrefs = entity.Attributes.Where(t => t.Value is EntityReference && (((EntityReference)t.Value).LogicalName == "systemuser" || ((EntityReference)t.Value).LogicalName == "teams")).ToList();
            foreach (var userattribute in userrefs)
            {
                    Entity newuser = SearchNewUser((EntityReference)userattribute.Value);

                    if (newuser == null)
                    {
                    entity.Attributes["addressused"] = ((EntityReference)userattribute.Value).Name;

                        entity.Attributes.Remove(userattribute.Key);
                    }
                    else
                    {
                        entity[userattribute.Key] = newuser.ToEntityReference();
                    }
            }
        }

        private void ResolveUser(Entity entity)
        {
            var userrefs = entity.Attributes.Where(t => t.Value is EntityReference && (((EntityReference)t.Value).LogicalName == "systemuser" || ((EntityReference)t.Value).LogicalName == "teams")).ToList();
            foreach (var userattribute in userrefs)
            {
                var c = Configuration.Mapping.SingleOrDefault(m => m.Target == userattribute.Key);
                if ((c != null && c.Automap))
                {
                    Entity newuser = SearchNewUser((EntityReference)userattribute.Value);

                    if (newuser == null)
                    {
                        entity.Attributes.Remove(userattribute.Key);
                    }
                    else
                    {
                        entity[userattribute.Key] = newuser.ToEntityReference();
                    }
                }
            }
        }

        private Entity SearchNewUser(EntityReference userref)
        {
            if (userref.Name == null)
                return null;

            Entity newuser = null;

            try
            {

                switch (userref?.LogicalName)
                {
                    case "team":
                        userref = teams.Entities.SingleOrDefault(t => t["name"].ToString() == userref.Name)?.ToEntityReference();
                        break;

                    case "systemuser":

                        newuser = users.Entities.SingleOrDefault(t => t.Contains("internalemailaddress") && t["internalemailaddress"].ToString().ToLower() == userref.Name.ToLower());
                        if (newuser != null) { break; }

                        newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == userref.Name.ToLower());
                        if (newuser != null) { break; }

                        var key = userref.Name.Replace(',', ' ');
                        newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == key.ToLower());
                        if (newuser != null) { break; }

                        if (!userref.Name.Contains('@')) { break; }

                        var name = userref.Name.Substring(0, userref.Name.IndexOf('@')).Split('.');

                        if (name.Length == 1)
                        {
                            newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == name[0].ToLower());
                            break;
                        }

                        if (name.Length != 2)
                        {
                            break;
                        }

                        key = name[0] + " " + name[1];
                        newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == key.ToLower());
                        if (newuser != null) { break; }

                        key = name[1] + " " + name[0];
                        newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == key.ToLower());
                        if (newuser != null) { break; }

                        key = name[0] + "," + name[1];
                        newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == key.ToLower());
                        if (newuser != null) { break; }

                        key = name[1] + "," + name[0];
                        newuser = users.Entities.SingleOrDefault(t => t.Contains("fullname") && t["fullname"].ToString().ToLower() == key.ToLower());
                        if (newuser != null) { break; }

                        userref = null;

                        break;
                }

            }
            catch
            {
            }

            return newuser;
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
                if (entity.LogicalName == "listmember")
                {
                    AddMemberListRequest addMemberListRequest = new AddMemberListRequest();
                    addMemberListRequest.ListId = ((EntityReference)entity["listid"]).Id;
                    addMemberListRequest.EntityId = ((EntityReference)entity["entityid"]).Id;
                    service.Execute(addMemberListRequest);
                }
                else
                {
                    try
                    {
                        entity.Id = service.Create(entity);
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(new TimeSpan(0,0,0,5));
                        entity.Id = service.Create(entity);
                    }
                }
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
                        try
                        {
                            service.Update(entity);
                        }
                        catch (Exception e)
                        {
                            Thread.Sleep(new TimeSpan(0, 0, 0, 5));
                            service.Update(entity);
                        }
                        
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
                if (ownerMustBeSet)
                {
                    service.SetOwnerOfEntity(entity.LogicalName, entity.Id, ownerid.LogicalName, ownerid.Id);
                    resolvedEntity["ownerid"] = ownerid;
                }

                var resolvedEntityStatuscode = resolvedEntity.Contains("statuscode") ? (OptionSetValue)resolvedEntity["statuscode"] : null;
                bool statusMustBeSet = EntityUpdateHandler.StatusMustBeSet(statuscode, resolvedEntityStatuscode, Configuration.SetStateMode);
                if (statusMustBeSet)
                {
                    service.SetStateOfEntity(entity.LogicalName, entity.Id, statecode, statuscode);
                    resolvedEntity["statecode"] = statecode;
                    resolvedEntity["statuscode"] = statuscode;
                }
            }
        }
    }
}
