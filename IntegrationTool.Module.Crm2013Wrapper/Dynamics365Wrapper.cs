using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.CrmWrapper
{
    public class Dynamics365Wrapper
    {
        public static IOrganizationService GetConnection(string connectionString)
        {
            return new CrmServiceClient(connectionString) as IOrganizationService;
        }
    }
}
