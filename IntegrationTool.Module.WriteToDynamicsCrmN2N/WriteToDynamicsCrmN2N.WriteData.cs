using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
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

        private EntityMetadata entity1Metadata;
        private EntityMetadata entity2Metadata;
        private RelationshipMetadataBase relationshipMetadata;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            this.reportProgress = reportProgress;

            reportProgress(new SimpleProgressReport("Connection to crm"));
            this.service = connection.GetConnection() as IOrganizationService;
            this.dataObject = dataObject;

            reportProgress(new SimpleProgressReport("Load required metadata from crm"));
            entity1Metadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.service, this.Configuration.Entity1Name);
            entity2Metadata = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(this.service, this.Configuration.Entity2Name.Split(';')[0]);
            relationshipMetadata = Crm2013Wrapper.Crm2013Wrapper.GetRelationshipMetadata(this.service, this.Configuration.Entity2Name.Split(';')[1]) as ManyToManyRelationshipMetadata;

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
                
                if(!this.existingEntities1.ContainsKey(joinKeyEntity1) || !this.existingEntities2.ContainsKey(joinKeyEntity2))
                {
                    continue;
                    // TODO Log, that one of the 2 entities could not be resolved!
                }

                if(this.Configuration.MultipleFoundMode == N2NMultipleFoundMode.None && (this.existingEntities1[joinKeyEntity1].Length > 1 || this.existingEntities2[joinKeyEntity2].Length > 1))
                {
                    continue;
                    // TODO Log, that more than one entities were resolved by this key
                }

                Guid entity1id = this.existingEntities1[joinKeyEntity1][0];
                Guid entity2id = this.existingEntities2[joinKeyEntity2][0];

                Crm2013Wrapper.Crm2013Wrapper.AssociateEntities(service, relationshipMetadata.SchemaName, entity1Metadata.LogicalName, entity1id,entity2Metadata.LogicalName, entity2id);

                if (StatusHelper.MustShowProgress(i, dataObject.Count) == true)
                {
                    reportProgress(new SimpleProgressReport("Processed " + (i + 1) + " of " + dataObject.Count + " many2many-records"));
                }
            }
        }
    }
}
