using IntegrationTool.DataMappingControl;
using IntegrationTool.Module.WriteToDynamicsCrmN2N;
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

            var defaultUnits = Test_Helpers.GetDefaultUnitGroup(service);

            Entity campaign = new Entity("campaign");
            campaign.Attributes.Add("name", "test prd_cmpgn 1");
            Guid campaignId = service.Create(campaign);

            
            Entity product = new Entity("product");
            product.Attributes.Add("name", "test prd_cmpgn 1");
            product.Attributes.Add("productnumber", "prd_cmpgn 1");
            product.Attributes.Add("defaultuomscheduleid", new EntityReference("uomschedule", defaultUnits.DefaultUnitGroupId));
            product.Attributes.Add("defaultuomid", new EntityReference("uom", defaultUnits.PrimaryUnitId));
            
            Guid productId = service.Create(product);

            WriteToDynamicsCrmN2NConfiguration writeToDynamicsCrmN2NConfig = new WriteToDynamicsCrmN2NConfiguration();
            writeToDynamicsCrmN2NConfig.MultipleFoundMode = N2NMultipleFoundMode.None;
            writeToDynamicsCrmN2NConfig.Entity1Name = "campaign";
            writeToDynamicsCrmN2NConfig.Entity1Mapping = new List<DataMapping>();
            writeToDynamicsCrmN2NConfig.Entity2Name = "product;campaignproduct_association";
            writeToDynamicsCrmN2NConfig.Entity2Mapping = new List<DataMapping>();
            // TODO --> Implement the test

            service.Delete("campaign", campaignId);
            service.Delete("product", productId);
        }
    }
}
