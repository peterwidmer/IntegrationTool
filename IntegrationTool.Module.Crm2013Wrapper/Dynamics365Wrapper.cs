using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace IntegrationTool.Module.CrmWrapper
{
    public class Dynamics365Wrapper
    {
        // this Connectionstring extension is used to garanee that the sdk is always creating a new Instance
        private const string RequirenewinstanceTrue = "RequireNewInstance = True;";

        public static IOrganizationService GetConnection(string connectionString)
        {
            return new CrmServiceClient(connectionString + RequirenewinstanceTrue);
        }
    }
}
