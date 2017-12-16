using IntegrationTool.Module.ConnectToSharepoint;
using IntegrationTool.SDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Sources
{
    [TestClass]
    public class Test_LoadFromSharepoint
    {
        private static Guid CRMCONNECTIONID = new Guid("4D3F2E27-71EC-4631-8E98-5915E99FCED2");
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            var configuration = new ConnectToSharepointConfiguration();
            configuration.AuthenticationType = SharepointAuthenticationType.SharepointOnline;


            connection = new ConnectToSharepoint() { Configuration = configuration };
        }

        [TestMethod]
        public void GeneralLoadTestSharepoint()
        {
        }
    }
}
