using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
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

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            this.reportProgress = reportProgress;

            reportProgress(new SimpleProgressReport("Connection to crm"));
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            this.service = new OrganizationService(crmConnection);
            this.dataObject = dataObject;
        }
    }
}
