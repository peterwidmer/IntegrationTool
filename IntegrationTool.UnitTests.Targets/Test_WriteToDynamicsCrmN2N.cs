using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.WriteToDynamicsCrmN2N;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
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
        private static IOrganizationService service;
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeWriteToDynamicsCrmN2N(TestContext context)
        {
            connection = Test_Helpers.GetDynamicsCrmConnection();
            var crmConnection = (CrmConnection)connection.GetConnection();
            service = new OrganizationService(crmConnection);
        }

        [TestMethod]
        public void Test_RelateContactsWithInvoice()
        {
            Entity contact = Test_Helpers.CreateDummyContact();
            Guid contactId = service.Create(contact);

            Entity invoice = Test_Helpers.CreateDummyInvoice();
            Guid invoiceId = service.Create(invoice);

            WriteToDynamicsCrmN2NConfiguration writeToDynamicsCrmN2NConfig = new WriteToDynamicsCrmN2NConfiguration();
            writeToDynamicsCrmN2NConfig.MultipleFoundMode = N2NMultipleFoundMode.None;
            writeToDynamicsCrmN2NConfig.Entity1Name = "contact";
            writeToDynamicsCrmN2NConfig.Entity1Mapping = new List<DataMapping>() { new DataMapping("FirstName", "firstname") };
            writeToDynamicsCrmN2NConfig.Entity2Name = "invoice;contactinvoices_association";
            writeToDynamicsCrmN2NConfig.Entity2Mapping = new List<DataMapping>() { new DataMapping("InvoiceNo", "invoicenumber") };
            writeToDynamicsCrmN2NConfig.ConfigurationId = Guid.NewGuid();
            writeToDynamicsCrmN2NConfig.SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID;
        }

        [TestMethod]
        public void Test_RelateProductsWithCampaigns()
        {
            var defaultUnits = Test_Helpers.GetDefaultUnitGroup(service);

            Entity campaign = new Entity("campaign");
            campaign.Attributes.Add("name", "test prd_cmpgn 1");
            Guid campaignId = service.Create(campaign);
            
            Entity product = new Entity("product");
            product.Attributes.Add("name", "test prd_cmpgn 1");
            product.Attributes.Add("productnumber", "prd_cmpgn 1");
            product.Attributes.Add("defaultuomscheduleid", new EntityReference("uomschedule", defaultUnits.DefaultUnitGroupId));
            product.Attributes.Add("defaultuomid", new EntityReference("uom", defaultUnits.PrimaryUnitId));
            product.Attributes.Add("quantitydecimal", 2);
            Guid productId = service.Create(product);

            Microsoft.Crm.Sdk.Messages.PublishProductHierarchyRequest publishProduct = new Microsoft.Crm.Sdk.Messages.PublishProductHierarchyRequest();
            publishProduct.Target = new EntityReference("product", productId);
            service.Execute(publishProduct);

            WriteToDynamicsCrmN2NConfiguration writeToDynamicsCrmN2NConfig = new WriteToDynamicsCrmN2NConfiguration();
            writeToDynamicsCrmN2NConfig.MultipleFoundMode = N2NMultipleFoundMode.None;
            writeToDynamicsCrmN2NConfig.Entity1Name = "campaign";
            writeToDynamicsCrmN2NConfig.Entity1Mapping = new List<DataMapping>() { new DataMapping("CampaigName", "name") };
            writeToDynamicsCrmN2NConfig.Entity2Name = "product;campaignproduct_association";
            writeToDynamicsCrmN2NConfig.Entity2Mapping = new List<DataMapping>() { new DataMapping("ProductNumber", "productnumber") };
            writeToDynamicsCrmN2NConfig.ConfigurationId = Guid.NewGuid();
            writeToDynamicsCrmN2NConfig.SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID;

            IDatastore dataObject = new IntegrationTool.SDK.DataObject();
            dataObject.AddColumn(new ColumnMetadata("CampaigName"));
            dataObject.AddColumn(new ColumnMetadata("ProductNumber"));

            dataObject.AddData(new object[] { "test prd_cmpgn 1", "prd_cmpgn 1" });

            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrmN2N)) as IModule;
            module.SetConfiguration(writeToDynamicsCrmN2NConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), dataObject, Test_Helpers.ReportProgressMethod);

            service.Delete("campaign", campaignId);
            service.Delete("product", productId);
        }
    }
}
