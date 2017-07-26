using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.Module.Crm2013Wrapper;
using IntegrationTool.Module.WriteToDynamicsCrm;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Targets.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Targets
{
    [TestClass]
    public class Test_WriteToDynamicsCrm
    {
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            connection = Test_Helpers.GetDynamicsCrmConnection();
        }

        [TestMethod]
        public void Test_ContactImport()
        {
            var organizationService = connection.GetConnection() as IOrganizationService;
            
            var accountName1 = Guid.NewGuid().ToString();
            var account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = organizationService.Create(account);

            var writeToCrmConfig = GetContactImportConfiguration();

            IDatastore contactDatastore = GetContactDataStore(accountName1);
            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrm)) as IModule;
            module.SetConfiguration(writeToCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), contactDatastore, Test_Helpers.ReportProgressMethod);

            var contactsId1001 = Crm2013Wrapper.RetrieveMultiple(organizationService, "contact", new ColumnSet("new_id"), new ConditionExpression("new_id", ConditionOperator.Equal, "1001"));
            Assert.AreEqual(1, contactsId1001.Count);

            // Prepare second run
            writeToCrmConfig.ImportMode = ImportMode.AllChangedValuesOnly;
            contactDatastore.SetValue(0, 2, "Baden");
            contactDatastore.AddData(new object[] { "Thomas", "Meyer", "New York", 1004, accountName1, "Active", new DateTime(1973, 04, 24) });

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), contactDatastore, Test_Helpers.ReportProgressMethod);
            var contactsId1004 = Crm2013Wrapper.RetrieveMultiple(organizationService, "contact", new ColumnSet("new_id"), new ConditionExpression("new_id", ConditionOperator.Equal, "1004"));
            Assert.AreEqual(1, contactsId1004.Count);

            organizationService.Delete("account", account1);
        }

        private static WriteToDynamicsCrmConfiguration GetContactImportConfiguration()
        {
            var writeToCrmConfig = new WriteToDynamicsCrmConfiguration()
            {
                EntityName = "contact",
                ImportMode = ImportMode.All,
                MultipleFoundMode = MultipleFoundMode.All,
                ConfigurationId = Guid.NewGuid(),
                SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID
            };

            writeToCrmConfig.PrimaryKeyAttributes.Add("new_id");
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "ID", Target = "new_id" });
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "FirstName", Target = "firstname" });
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "LastName", Target = "lastname" });
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "Status", Target = "statuscode" });
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "Birthdate", Target = "birthdate" });
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "City", Target = "address1_city" });
            writeToCrmConfig.RelationMapping.Add(new Module.WriteToDynamicsCrm.SDK.RelationMapping()
            {
                EntityName = "account",
                LogicalName = "parentcustomerid",
                Mapping = new List<DataMappingControl.DataMapping>() { new DataMappingControl.DataMapping()
                    {
                        Source ="CompanyName",
                        Target = "name"
                    } }
            });

            writeToCrmConfig.PicklistMapping.Add(new Module.WriteToDynamicsCrm.SDK.PicklistMapping()
            {
                LogicalName = "statuscode",
                MappingType = PicklistMappingType.Manual,
                Mapping = new List<DataMappingControl.DataMapping>() 
                { 
                    new DataMappingControl.DataMapping("Active", "1"),
                    new DataMappingControl.DataMapping("Inactive", "2")
                }
            });
            return writeToCrmConfig;
        }

        private IDatastore GetContactDataStore(string accountName1)
        {
            IDatastore dataObject = DataStoreFactory.GetDatastore();
            dataObject.AddColumn(new ColumnMetadata("FirstName"));
            dataObject.AddColumn(new ColumnMetadata("LastName"));
            dataObject.AddColumn(new ColumnMetadata("City"));
            dataObject.AddColumn(new ColumnMetadata("ID"));
            dataObject.AddColumn(new ColumnMetadata("CompanyName"));
            dataObject.AddColumn(new ColumnMetadata("Status"));
            dataObject.AddColumn(new ColumnMetadata("Birthdate"));

            dataObject.AddData(new object[] { "Peter", "Widmer", "Wettingen", 1001, accountName1, "Active", new DateTime(1980, 06, 23) });
            dataObject.AddData(new object[] { "Joachim 2", "Suter", "Dättwil", 1002, accountName1, "Inactive", new DateTime(2004, 12, 03) });
            dataObject.AddData(new object[] { "James", "Brown", "London", 1003, null, "Active", null });
            // Doublekey to test it works too
            dataObject.AddData(new object[] { "Peter", "Widmer", "Wettingen", 1001, accountName1, "Active", new DateTime(1980, 06, 23) });

            return dataObject;
        }

        [TestMethod]
        public void Test_CaseImport()
        {
            var organizationService = connection.GetConnection() as IOrganizationService;

            string accountName1 = Guid.NewGuid().ToString();
            Entity account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = organizationService.Create(account);

            var writeToCrmConfig = new WriteToDynamicsCrmConfiguration()
            {
                ConfigurationId = Guid.NewGuid(),
                SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID,
                EntityName = "incident",                
                ImportMode = Module.WriteToDynamicsCrm.SDK.Enums.ImportMode.All,
                MultipleFoundMode = Module.WriteToDynamicsCrm.SDK.Enums.MultipleFoundMode.All,
            };

            writeToCrmConfig.PrimaryKeyAttributes.Add("ticketnumber");
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "CaseID", Target = "ticketnumber" });
            writeToCrmConfig.Mapping.Add(new DataMapping() { Source = "CaseTitle", Target = "title" });

            writeToCrmConfig.RelationMapping.Add(new Module.WriteToDynamicsCrm.SDK.RelationMapping()
            {
                EntityName = "account",
                LogicalName = "customerid",
                Mapping = new List<DataMapping>() { new DataMapping()
                    {
                        Source ="CompanyName",
                        Target = "name"
                    } }
            });

            IDatastore dataObject = DataStoreFactory.GetDatastore();
            dataObject.AddColumn(new ColumnMetadata("CaseID"));
            dataObject.AddColumn(new ColumnMetadata("CaseTitle"));
            dataObject.AddColumn(new ColumnMetadata("CompanyName"));

            dataObject.AddData(new object[] { "CA-100", "101 Title", accountName1 });
            dataObject.AddData(new object[] { "CA-101", "Anöther Title", null });
            dataObject.AddData(new object[] { "CA-102", "A'Title\"chen", accountName1 });

            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrm)) as IModule;
            module.SetConfiguration(writeToCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), dataObject, Test_Helpers.ReportProgressMethod);

            organizationService.Delete("account", account1);
        }
    }
}
