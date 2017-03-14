using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.Module.DeleteInDynamicsCrm;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
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
    public class Test_DeleteInDynamicsCrm
    {
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            connection = Test_Helpers.GetDynamicsCrmConnection();
        }

        [TestMethod]
        public void Test_AccountDeletion()
        {
            var organizationService = connection.GetConnection() as IOrganizationService;

            // Create dummy account, which shall be deleted
            string accountName1 = Guid.NewGuid().ToString();
            Entity account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = organizationService.Create(account);

            DeleteInDynamicsCrmConfiguration deleteInCrmConfig = new DeleteInDynamicsCrmConfiguration();
            deleteInCrmConfig.EntityName = "account";
            deleteInCrmConfig.MultipleFoundMode = DeleteInCrmMultipleFoundMode.DeleteAll;
            deleteInCrmConfig.DeleteMapping.Add(new DataMapping() { Source = "CompanyName", Target = "name" });

            IDatastore dataObject = DataStoreFactory.GetDatastore();
            dataObject.AddColumn(new ColumnMetadata("CompanyName"));

            dataObject.AddData(new object[] { accountName1 });

            IModule module = Activator.CreateInstance(typeof(DeleteInDynamicsCrm)) as IModule;
            module.SetConfiguration(deleteInCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), dataObject, ReportProgressMethod);

        }

        private void ReportProgressMethod(SimpleProgressReport progress) { }   
    }
}
