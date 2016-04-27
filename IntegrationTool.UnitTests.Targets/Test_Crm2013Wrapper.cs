using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.SDK;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using IntegrationTool.UnitTests.Targets.Properties;
using IntegrationTool.Module.Crm2013Wrapper;
using System.Linq;

namespace IntegrationTool.UnitTests.Targets
{
    [TestClass]
    public class Test_Crm2013Wrapper
    {
        private static OrganizationService orgServiceInstance;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            ConnectToDynamicsCrmConfiguration configuration = new ConnectToDynamicsCrmConfiguration();
            configuration.ConnectionString = Settings.Default.CrmConnectionString;

            IConnection connection = new ConnectToDynamicsCrm() { Configuration = configuration };
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();

            orgServiceInstance = new OrganizationService(crmConnection);
        }

        [TestMethod]
        public void GetEntityMetadata_ExistingEntity()
        {
            var accountEntityMetadata = Crm2013Wrapper.GetEntityMetadata(orgServiceInstance, "account");
        }

        [TestMethod]
        public void GetEntityMetadata_GetAllEntities()
        {
            var entities = Crm2013Wrapper.GetAllEntities(orgServiceInstance);
            if(entities.Where(t=> t.Name == "account").Count() == 0)
            {
                Assert.Fail("Could not find account in entitylist");
            }
        }

        [TestMethod]
        public void GetEntityMetadata_GetRelationshipMetata()
        {

        }
    }
}
