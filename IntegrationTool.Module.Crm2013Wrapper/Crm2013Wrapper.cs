using IntegrationTool.SDK.GenericClasses;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using IntegrationTool.SDK;

namespace IntegrationTool.Module.Crm2013Wrapper
{
    public class Crm2013Wrapper
    {
        // this Connectionstring extension is used to garanee that the sdk is always creating a new Instance
        private const string RequirenewinstanceTrue = "; RequireNewInstance = True;";

        public static IOrganizationService GetConnection(string connectionString)
        {
            var crmServiceClient = new CrmServiceClient(connectionString + RequirenewinstanceTrue);
            return crmServiceClient.OrganizationWebProxyClient != null ? 
                (IOrganizationService)crmServiceClient.OrganizationWebProxyClient : 
                (IOrganizationService)crmServiceClient.OrganizationServiceProxy;
        }

        public static EntityMetadata GetEntityMetadata(IOrganizationService service, string entityName)
        {            
            var retrieveEntityRequest = new RetrieveEntityRequest()
            {
                EntityFilters = EntityFilters.All,
                LogicalName = entityName,
                RetrieveAsIfPublished = false
            };

            try
            {
                var retrieveEntityResponse = (RetrieveEntityResponse)service.Execute(retrieveEntityRequest);
                return retrieveEntityResponse.EntityMetadata;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception("Error on loading metadata: " + ex.Detail.Message);
            }
        }

        public static RelationshipMetadataBase GetRelationshipMetadata(IOrganizationService service, string relationshipName)
        {
            var retrieveRelationshipRequest = new RetrieveRelationshipRequest() { Name = relationshipName };

            try
            {
                var retrieveRelationshipResponse = (RetrieveRelationshipResponse)service.Execute(retrieveRelationshipRequest);

                return retrieveRelationshipResponse.RelationshipMetadata;
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new Exception("Error on loading metadata: " + ex.Detail.Message);
            }
        }

        public static List<NameDisplayName> GetAllEntities(IOrganizationService service)
        {
            RetrieveAllEntitiesRequest req = new RetrieveAllEntitiesRequest();
            req.EntityFilters = EntityFilters.Entity;
            req.RetrieveAsIfPublished = true;

            RetrieveAllEntitiesResponse response = (RetrieveAllEntitiesResponse)service.Execute(req);

            List<NameDisplayName> list = new List<NameDisplayName>();
            foreach (var item in response.EntityMetadata)
            {
                string displayName = item.DisplayName.LocalizedLabels.Count > 0 ?
                                                item.DisplayName.LocalizedLabels[0].Label :
                                                item.LogicalName;
                list.Add(new NameDisplayName(item.LogicalName, displayName));
            }

            return list.OrderBy(t => t.Name).ToList();
        }

        public static List<NameDisplayName> GetPicklistValuesOfPicklistAttributeMetadata(AttributeMetadata attributeMetadata)
        {
            EnumAttributeMetadata picklistAttributeMetadata = attributeMetadata as EnumAttributeMetadata;
            
            if(picklistAttributeMetadata == null)
            {
                throw new ArgumentException("Type of argument must be PicklistAttributeMetadata", "attributeMetadata");
            }

            List<NameDisplayName> list = new List<NameDisplayName>();
            foreach(var option in picklistAttributeMetadata.OptionSet.Options)
            {
                list.Add(new NameDisplayName(option.Value.Value.ToString(), option.Label.LocalizedLabels[0].Label));
            }

            return list.OrderBy(t => t.DisplayName).ToList();
        }

        public static List<NameDisplayName> GetAllAttributesOfEntity(EntityMetadata entityMetadata)
        {
            List<NameDisplayName> attributeList = new List<NameDisplayName>();
            foreach (var attribute in entityMetadata.Attributes)
                //.Where(attribute => (attribute.IsValidForCreate.HasValue && (bool)attribute.IsValidForCreate) || (attribute.IsValidForUpdate.HasValue && (bool)attribute.IsValidForUpdate)))
            {
                if(attribute.AttributeType.Value == AttributeTypeCode.Virtual || attribute.AttributeType.Value == AttributeTypeCode.State)
                {
                    continue;
                }

                string displayName = attribute.DisplayName.LocalizedLabels.Count > 0 ?
                                                attribute.DisplayName.LocalizedLabels[0].Label :
                                                attribute.LogicalName;
                attributeList.Add(new NameDisplayName(attribute.LogicalName, displayName));
            }

            return attributeList.OrderBy(t => t.Name).ToList();
        }

        public static DataCollection<Entity> RetrieveMultiple(IOrganizationService service, string entityName, ColumnSet columnSet, ConditionExpression condition, LogicalOperator logicalOperator = LogicalOperator.And)
        {
            List<ConditionExpression> conditions = new List<ConditionExpression>() { condition };
            return RetrieveMultiple(service, entityName, columnSet, conditions, logicalOperator);
        }

        public static DataCollection<Entity> RetrieveMultiple(IOrganizationService service, string entityName, ColumnSet columnSet, List<ConditionExpression> conditions, LogicalOperator logicalOperator = LogicalOperator.And)
        {
            FilterExpression filterExpression = new FilterExpression(logicalOperator);
            filterExpression.Conditions.AddRange(conditions);

            return RetrieveMultiple(service, entityName, columnSet, filterExpression);
        }

        public static DataCollection<Entity> RetrieveMultiple(IOrganizationService service, string entityName, ColumnSet columnSet, FilterExpression filterExpression)
        {
            QueryExpression query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(true);
            query.Criteria = filterExpression;

            EntityCollection result = service.RetrieveMultiple(query);

            return result.Entities;
        }

        public static string GetAttributeIdentifierStringValue(Entity entity, AttributeMetadata attributeMetadata)
        {
            if (entity.Contains(attributeMetadata.LogicalName) == false || entity[attributeMetadata.LogicalName] == null)
            {
                return null;
            }

            switch (attributeMetadata.AttributeType.Value)
            {
                case AttributeTypeCode.BigInt:
                case AttributeTypeCode.Boolean:
                case AttributeTypeCode.DateTime:
                case AttributeTypeCode.Decimal:
                case AttributeTypeCode.Double:
                case AttributeTypeCode.EntityName:
                case AttributeTypeCode.Integer:
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.String:
                case AttributeTypeCode.Uniqueidentifier:
                    return entity[attributeMetadata.LogicalName].ToString();

                case AttributeTypeCode.Customer:
                case AttributeTypeCode.Lookup:
                case AttributeTypeCode.Owner:
                    return ((EntityReference)entity[attributeMetadata.LogicalName]).Id.ToString();

                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist:
                    return ((OptionSetValue)entity[attributeMetadata.LogicalName]).Value.ToString();

                case AttributeTypeCode.Money:
                    return ((Money)entity[attributeMetadata.LogicalName]).Value.ToString();

                default:
                    throw new Exception("Could not get string-value for " + attributeMetadata.LogicalName + ". Type is " + attributeMetadata.AttributeType.Value.ToString());
            }
        }

        public static void AssociateEntities(IOrganizationService service, string relationshipName, string entityName1, Guid entity1id, string entityName2, Guid entity2id)
        {
            if (entityName1 == "campaign" || entityName2 == "campaign")
            {
                AddItemCampaignRequest addItemCampaignRequest = new AddItemCampaignRequest();
                if (entityName1 == "campaign")
                {
                    addItemCampaignRequest.CampaignId = entity1id;
                    addItemCampaignRequest.EntityName = entityName2;
                    addItemCampaignRequest.EntityId = entity2id;
                }
                else
                {
                    addItemCampaignRequest.CampaignId = entity2id;
                    addItemCampaignRequest.EntityName = entityName1;
                    addItemCampaignRequest.EntityId = entity1id;
                }
                service.Execute(addItemCampaignRequest);
            }
            else
            {
                AssociateEntitiesRequest associateEntitiesRequest = new AssociateEntitiesRequest();
                associateEntitiesRequest.RelationshipName = relationshipName;
                associateEntitiesRequest.Moniker1 = new EntityReference(entityName1, entity1id);
                associateEntitiesRequest.Moniker2 = new EntityReference(entityName2, entity2id);

                service.Execute(associateEntitiesRequest);
            }
        }

        public static void DeleteRecordInCrm(IOrganizationService service, string entityName, Guid entityId)
        {
            service.Delete(entityName, entityId);
        }

        public delegate void RetrievedEntityCollectionMethod(EntityCollection retrievedEntityCollection);

        public static void ExecuteFetchXml(IOrganizationService service, string fetchXml, RetrievedEntityCollectionMethod retrievedEntityCollection, ReportProgressMethod reportProgress, bool mappingPreview)
        {
            int pageNumber = 1;
            string pagingCookie = null;
            int fetchCount = 250;

            if (mappingPreview)
            {
                var conversionRequest = new FetchXmlToQueryExpressionRequest
                {
                    FetchXml = fetchXml
                };

                var conversionResponse = (FetchXmlToQueryExpressionResponse)service.Execute(conversionRequest);

                // Use the newly converted query expression to make a retrieve multiple
                // request to Microsoft Dynamics CRM.
                QueryExpression queryExpression = conversionResponse.Query;

                var retrievesEntityRequest = new RetrieveEntityRequest
                {
                    EntityFilters = EntityFilters.All,
                    LogicalName = queryExpression.EntityName,
                };
                var response = (RetrieveEntityResponse)service.Execute(retrievesEntityRequest);
                EntityCollection dummy = new EntityCollection();

                Entity entity = new Entity(queryExpression.EntityName);
                foreach (var item in response.EntityMetadata.Attributes)
                {
                    entity.Attributes[item.LogicalName] = "Dummy";
                }
                dummy.Entities.Add(entity);
                retrievedEntityCollection(dummy);
            }


            while (true)
            {
                string pagingString = BuildFetchXmlCookie(pageNumber, pagingCookie, fetchCount);

                RetrieveMultipleRequest retrieveMultipleRequest = new RetrieveMultipleRequest();

                string xml = CreateXml(fetchXml, pagingCookie, pageNumber, fetchCount);

                retrieveMultipleRequest.Query = new FetchExpression(xml);

                EntityCollection retrievedEntities = ((RetrieveMultipleResponse)service.Execute(retrieveMultipleRequest)).EntityCollection;
                retrievedEntityCollection(retrievedEntities);

                if(retrievedEntities.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = retrievedEntities.PagingCookie;

                    if (mappingPreview && pageNumber > 5)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

                reportProgress(new SimpleProgressReport("fetching entities... Page Nummber: " + pageNumber));
            }
        }

        public string ExtractNodeValue(XmlNode parentNode, string name)
        {
            XmlNode childNode = parentNode.SelectSingleNode(name);

            if (null == childNode)
            {
                return null;
            }
            return childNode.InnerText;
        }

        public string ExtractAttribute(XmlDocument doc, string name)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;
            XmlAttribute attr = (XmlAttribute)attrs.GetNamedItem(name);
            if (null == attr)
            {
                return null;
            }
            return attr.Value;
        }

        public static string CreateXml(string xml, string cookie, int page, int count)
        {
            StringReader stringReader = new StringReader(xml);
            XmlTextReader reader = new XmlTextReader(stringReader);

            // Load document
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            return CreateXml(doc, cookie, page, count);
        }

        public static string CreateXml(XmlDocument doc, string cookie, int page, int count)
        {
            XmlAttributeCollection attrs = doc.DocumentElement.Attributes;

            if (cookie != null)
            {
                XmlAttribute pagingAttr = doc.CreateAttribute("paging-cookie");
                pagingAttr.Value = cookie;
                attrs.Append(pagingAttr);
            }

            XmlAttribute pageAttr = doc.CreateAttribute("page");
            pageAttr.Value = System.Convert.ToString(page);
            attrs.Append(pageAttr);

            XmlAttribute countAttr = doc.CreateAttribute("count");
            countAttr.Value = System.Convert.ToString(count);
            attrs.Append(countAttr);

            StringBuilder sb = new StringBuilder(1024);
            StringWriter stringWriter = new StringWriter(sb);

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            doc.WriteTo(writer);
            writer.Close();

            return sb.ToString();
        }

        public static string BuildFetchXmlCookie(int pageNumber, string pagingCookie, int fetchCount)
        {
            return string.Format("page='{0}' paging-cookie='{1}' count='{2}'", pageNumber, pagingCookie, fetchCount);
        }
    }
}
