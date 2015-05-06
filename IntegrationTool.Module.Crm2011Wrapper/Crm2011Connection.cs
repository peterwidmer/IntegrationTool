using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.Crm2011Wrapper
{
    public class Crm2011Connection
    {
        public object GetConnection()
        {
            CrmConnection CrmConnectionInstance = new CrmConnection(""); // TODO Implement configuration
            OrganizationService OrgServiceInstance = new OrganizationService(CrmConnectionInstance);

            return OrgServiceInstance;
        }
        
    }
}
