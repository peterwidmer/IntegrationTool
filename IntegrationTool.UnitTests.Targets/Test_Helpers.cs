using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.SDK;
using IntegrationTool.UnitTests.Targets.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Targets
{
    public class Test_Helpers
    {
        public static Guid CRMCONNECTIONID = new Guid("4D3F2E27-71EC-4631-8E98-5915E99FCED2");

        public static IConnection GetDynamicsCrmConnection()
        {
            ConnectToDynamicsCrmConfiguration configuration = new ConnectToDynamicsCrmConfiguration();
            configuration.ConnectionString = Settings.Default.CrmConnectionString;

            return new ConnectToDynamicsCrm() { Configuration = configuration };
        }
    }
}
