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

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
    public partial class DeleteInDynamicsCrm
    {
        private IOrganizationService service = null;
        private EntityMetadata entityMetaData;
        private ReportProgressMethod reportProgress;
        private IDatastore dataObject;

        private Dictionary<string, Guid[]> existingEntityRecords;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            this.reportProgress = reportProgress;

            reportProgress(new SimpleProgressReport("Connection to crm"));
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            this.service = new OrganizationService(crmConnection);
            this.dataObject = dataObject;

            reportProgress(new SimpleProgressReport("Load " + this.Configuration.EntityName + " metadata"));
            this.entityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, "list");

            reportProgress(new SimpleProgressReport("Resolve existing entityrecords"));
            JoinResolver entityResolver = new JoinResolver(this.service, entityMetaData, this.Configuration.DeleteMapping);
            this.existingEntityRecords = entityResolver.BuildMassResolverIndex();

            for (int i = 0; i < this.dataObject.Count; i++)
            {
                string joinKey = JoinResolver.BuildExistingCheckKey(this.dataObject[i], this.Configuration.DeleteMapping, this.dataObject.Metadata);
                if (existingEntityRecords.ContainsKey(joinKey))
                {
                }
            }
        }
    }
}
