using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.Module.DeleteInDynamicsCrm;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Targets.Properties;
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
    class Test_DeleteInDynamicsCrm
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
        public void Test_AccountDeletion()
        {
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            IOrganizationService service = new OrganizationService(crmConnection);

            // Create dummy account, which shall be deleted
            string accountName1 = Guid.NewGuid().ToString();
            Entity account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = service.Create(account);

            DeleteInDynamicsCrmConfiguration deleteInCrmConfig = new DeleteInDynamicsCrmConfiguration();
            deleteInCrmConfig.EntityName = "contact";
            deleteInCrmConfig.DeleteMapping.Add(new DataMapping() { Source = "CompanyName", Target = "name" });

            IDatastore dataObject = new IntegrationTool.SDK.DataObject();
            dataObject.AddColumnMetadata(new ColumnMetadata(0, "CompanyName"));

            dataObject.AddData(new object[] { accountName1 });

            IModule module = Activator.CreateInstance(typeof(DeleteInDynamicsCrm)) as IModule;
            module.SetConfiguration(deleteInCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), dataObject, ReportProgressMethod);

            service.Delete("account", account1);
        }

        private void ReportProgressMethod(SimpleProgressReport progress) { }   
    }
}
