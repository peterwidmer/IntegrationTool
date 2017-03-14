using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.SDK;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using IntegrationTool.UnitTests.Targets.Properties;
using IntegrationTool.Module.Crm2013Wrapper;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace IntegrationTool.UnitTests.Targets
{
    [TestClass]
    public class Test_Crm2013Wrapper
    {
        private static IOrganizationService organizationService;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            ConnectToDynamicsCrmConfiguration configuration = new ConnectToDynamicsCrmConfiguration();
            configuration.ConnectionString = Settings.Default.CrmConnectionString;

            IConnection connection = new ConnectToDynamicsCrm() { Configuration = configuration };
            organizationService = connection.GetConnection() as IOrganizationService;
        }

        [TestMethod]
        public void GetEntityMetadata_ExistingEntity()
        {
            var accountEntityMetadata = Crm2013Wrapper.GetEntityMetadata(organizationService, "account");
        }

        [TestMethod]
        public void GetEntityMetadata_GetAllEntities()
        {
            var entities = Crm2013Wrapper.GetAllEntities(organizationService);
            if(entities.Where(t=> t.Name == "account").Count() == 0)
            {
                Assert.Fail("Could not find account in entitylist");
            }
        }

        [TestMethod]
        public void GetEntityMetadata_GetRelationshipMetata()
        {
            var relationShipMetadata = Crm2013Wrapper.GetRelationshipMetadata(organizationService, "contactinvoices_association");
        }
    }
}
