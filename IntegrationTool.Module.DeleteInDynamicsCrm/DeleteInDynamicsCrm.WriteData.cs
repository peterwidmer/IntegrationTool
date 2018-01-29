using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.Module.DeleteInDynamicsCrm.Logging;
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

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
    public partial class DeleteInDynamicsCrm
    {
        private IOrganizationService service = null;
        private EntityMetadata entityMetaData;
        private IDatastore dataObject;

        private Dictionary<string, Guid[]> existingEntityRecords;

        private Logger logger = null;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            reportProgress(new SimpleProgressReport("Building logging database"));
            this.logger = new Logger(databaseInterface);
            this.logger.InitializeDatabase();

            reportProgress(new SimpleProgressReport("Connection to crm"));
            this.service = connection.GetConnection() as IOrganizationService;
            this.dataObject = dataObject;

            reportProgress(new SimpleProgressReport("Load " + this.Configuration.EntityName + " metadata"));
            this.entityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, this.Configuration.EntityName);

            reportProgress(new SimpleProgressReport("Resolve existing entityrecords"));
            JoinResolver entityResolver = new JoinResolver(this.service, entityMetaData, this.Configuration.DeleteMapping);
            this.existingEntityRecords = entityResolver.BuildMassResolverIndex();

            for (int i = 0; i < this.dataObject.Count; i++)
            {
                string joinKey = JoinResolver.BuildExistingCheckKey(this.dataObject[i], this.Configuration.DeleteMapping, this.dataObject.Metadata);
                if (existingEntityRecords.ContainsKey(joinKey))
                {
                    var existingRecordIds = existingEntityRecords[joinKey];
                    if (existingRecordIds.Length == 1 || this.Configuration.MultipleFoundMode == DeleteInCrmMultipleFoundMode.DeleteAll)
                    {
                        string entityIds = string.Empty;
                        string deletionFaults = string.Empty;

                        foreach (Guid entityId in existingRecordIds)
                        {
                            entityIds += entityId.ToString() + ",";
                            try
                            {
                                Crm2013Wrapper.Crm2013Wrapper.DeleteRecordInCrm(this.service, this.Configuration.EntityName, entityId);
                            }
                            catch (FaultException<OrganizationServiceFault> ex)
                            {
                                deletionFaults += ex.Detail.Message + "\n";
                            }
                        }
                        entityIds = entityIds.TrimEnd(',');
                        logger.AddRecord(i, joinKey, entityIds, deletionFaults);
                    }
                    else
                    {
                        // TODO Log that multiplerecords were found but none deleted
                    }

                }
                else
                {
                    // TODO Log that no record was found to delete
                }

            }
        }
    }
}
