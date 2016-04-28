using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrmN2N
{
    public partial class WriteToDynamicsCrmN2N
    {
        private IOrganizationService service = null;
        private ReportProgressMethod reportProgress;
        private IDatastore dataObject;

        private Dictionary<string, Guid[]> existingEntities1;
        private Dictionary<string, Guid[]> existingEntities2;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            this.reportProgress = reportProgress;

            reportProgress(new SimpleProgressReport("Connection to crm"));
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            this.service = new OrganizationService(crmConnection);
            this.dataObject = dataObject;

            reportProgress(new SimpleProgressReport("Load required metadata from crm"));
            var entity1Metadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.service, this.Configuration.Entity1Name);
            var entity2Metadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.service, this.Configuration.Entity2Name.Split(';')[0]);
            var relationshipMetadata = Crm2013Wrapper.Crm2013Wrapper.GetRelationshipMetadata(this.service, this.Configuration.Entity2Name.Split(';')[1]) as ManyToManyRelationshipMetadata;

            reportProgress(new SimpleProgressReport("Cache keys of existing " + entity1Metadata.LogicalName + "-records"));
            var entity1Resolver = new JoinResolver(this.service, entity1Metadata, this.Configuration.Entity1Mapping);
            this.existingEntities1 = entity1Resolver.BuildMassResolverIndex();

            reportProgress(new SimpleProgressReport("Cache keys of existing " + entity2Metadata.LogicalName + "-records"));
            var entity2Resolver = new JoinResolver(this.service, entity2Metadata, this.Configuration.Entity2Mapping);
            this.existingEntities2 = entity2Resolver.BuildMassResolverIndex();

            RelateEntities();
        }

        public void RelateEntities()
        {
            for (int i = 0; i < this.dataObject.Count; i++)
            {
                string joinKeyEntity1 = JoinResolver.BuildExistingCheckKey(this.dataObject[i], this.Configuration.Entity1Mapping, this.dataObject.Metadata);
                string joinKeyEntity2 = JoinResolver.BuildExistingCheckKey(this.dataObject[i], this.Configuration.Entity2Mapping, this.dataObject.Metadata);
            }
        }
    }
}
