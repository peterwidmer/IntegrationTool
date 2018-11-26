using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.CrmWrapper
{
    public static class Dynamics365ExtensionMethods
    {
        public static Dictionary<string, AttributeMetadata> GetAttributeMetadata(this EntityMetadata entityMetadata)
        {
            var attributeMetadataDictionary = new Dictionary<string, AttributeMetadata>();
            foreach (AttributeMetadata attributeMetadata in entityMetadata.Attributes)
            {
                attributeMetadataDictionary.Add(attributeMetadata.LogicalName, attributeMetadata);
            }

            return attributeMetadataDictionary;
        }

        public static void SetOwnerOfEntity(this IOrganizationService service, string entityName, Guid entityId, string assigneeEntity, Guid assigneeId)
        {
            AssignRequest assignRequest = new AssignRequest();
            assignRequest.Assignee = new EntityReference(assigneeEntity, assigneeId);
            assignRequest.Target = new EntityReference(entityName, entityId);

            service.Execute(assignRequest);
        }

        public static void SetStateOfEntity(this IOrganizationService service, string entityName, Guid entityId, OptionSetValue statecode, OptionSetValue statuscode)
        {
            if (statecode == null || statecode == null)
            {
                throw new Exception("could not set status... please check the status mapping");
            }

            SetStateRequest setStateRequest = new SetStateRequest();
            setStateRequest.EntityMoniker = new EntityReference(entityName, entityId);
            setStateRequest.State = statecode;
            setStateRequest.Status = statuscode;

            service.Execute(setStateRequest);
        }
    }
}
