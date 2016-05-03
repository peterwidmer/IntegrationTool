using IntegrationTool.SDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Targets
{
    [TestClass]
    public class Test_WriteToDynamicsCrmN2N
    {
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeWriteToDynamicsCrmN2N(TestContext context)
        {
            connection = Test_Helpers.GetDynamicsCrmConnection();
        }

        [TestMethod]
        public void Test_RelateProductsWithCampaigns()
        {
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            IOrganizationService service = new OrganizationService(crmConnection);

            Entity campaign = new Entity("campaign");
            campaign.Attributes.Add("name", "test prd_cmpgn 1");
            Guid campaignId = service.Create(campaign);

            Entity product = new Entity("product");
            product.Attributes.Add("name", "test prd_cmpgn 1");
            Guid productId = service.Create(product);

            // TODO --> Implement the test

            service.Delete("campaign", campaignId);
            service.Delete("product", productId);
        }
    }
}
