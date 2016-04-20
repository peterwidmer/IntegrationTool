using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.SDK;
using IntegrationTool.Module.LoadFromDynamicsCrm;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Sources.Properties;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Client.Services;

namespace IntegrationTool.UnitTests.Sources
{
    [TestClass]
    public class Test_LoadFromDynamicsCRM
    {
        private static Guid CRMCONNECTIONID = new Guid("4D3F2E27-71EC-4631-8E98-5915E99FCED2");
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            ConnectToDynamicsCrmConfiguration configuration = new ConnectToDynamicsCrmConfiguration();
            configuration.ConnectionString = Settings.Default.CrmConnectionString;

            connection = new ConnectToDynamicsCrm() { Configuration = configuration };
        }

        [TestMethod]
        public void GeneralLoadTestDynamicsCRM()
        {
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            IOrganizationService service = new OrganizationService(crmConnection);
            
            Guid account1 = CreateAccount1(service);

            string contactName1 = Guid.NewGuid().ToString();
            Guid contact1 = CreateContact1(service, contactName1, account1);

            LoadFromDynamicsCrmConfiguration configuration = new LoadFromDynamicsCrmConfiguration();
            configuration.QueryType = DynamicsCrmQueryType.ExecuteFetchXml;
            configuration.FetchXml = GetTestFetchXml(contactName1);

            IDataSource loadFromDynamicsCrm = new LoadFromDynamicsCrm() { Configuration = configuration };

            IDatastore dataObject = new DataObject();
            loadFromDynamicsCrm.LoadData(connection, dataObject, ReportProgressMethod);

            var firstObject = dataObject[0];
            Assert.AreEqual<string>(contactName1, firstObject[dataObject.Metadata["firstname"].ColumnIndex].ToString());

            service.Delete("contact", contact1);
            service.Delete("account", account1);

            ((OrganizationService)service).Dispose();
        }

        private string GetTestFetchXml(string contactName1)
        {
            return "<fetch version=\"1.0\" output-format=\"xml-platform\" mapping=\"logical\" distinct=\"false\">" +
                    "<entity name=\"contact\">" +
                    "<attribute name=\"firstname\" />" +
                    "<attribute name=\"contactid\" />" +
                    "<attribute name=\"parentcustomerid\"/>" +
                    "<attribute name=\"leadsourcecode\"/>" +
                    "<order attribute=\"fullname\" descending=\"false\" />" +
                    "<filter type=\"and\">" +
                    "<condition attribute=\"firstname\" operator=\"eq\" value=\"" + contactName1 +"\" />" +
                    "</filter>" +
                    "<link-entity name=\"account\" from=\"accountid\" to=\"parentcustomerid\" visible=\"false\" alias=\"account_alias\">" +
                    "<attribute name=\"name\" />" +
                    "</link-entity>" +
                    "</entity>" +
                    "</fetch>";
        }

        private static Guid CreateContact1(IOrganizationService service, string contactName1, Guid account1)
        {
            
            Entity contact = new Entity("contact");
            contact.Attributes.Add("firstname", contactName1);
            contact.Attributes.Add("parentcustomerid", new EntityReference("account", account1));
            Guid contact1 = service.Create(contact);
            return contact1;
        }

        private static Guid CreateAccount1(IOrganizationService service)
        {
            string accountName1 = Guid.NewGuid().ToString();
            Entity account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = service.Create(account);
            return account1;
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
        }
    }
}
