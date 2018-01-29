using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.Module.Crm2013Wrapper;
using IntegrationTool.Module.WriteToDynamicsCrm;
using IntegrationTool.Module.WriteToDynamicsCrm.Execution;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Targets.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private static IOrganizationService organizationService;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            connection = Test_Helpers.GetDynamicsCrmConnection();
            organizationService = connection.GetConnection() as IOrganizationService;
        }

        [TestMethod]
        public void Test_ContactImport()
        {            
            var accountName1 = Guid.NewGuid().ToString();
            var account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = organizationService.Create(account);


            // FIRST RUN
            var writeToCrmConfig = GetContactImportConfiguration();

            IDatastore contactDatastore = GetContactDataStore(accountName1);
            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrm)) as IModule;
            module.SetConfiguration(writeToCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), contactDatastore, Test_Helpers.ReportProgressMethod);

            var contactsId1001 = Crm2013Wrapper.RetrieveMultiple(organizationService, "contact", new ColumnSet(true), new ConditionExpression("new_id", ConditionOperator.Equal, "1001"));
            Assert.AreEqual(1, contactsId1001.Count);
            var contact1001 = contactsId1001.First();
            Assert.AreEqual("James Test", ((EntityReference)contact1001["ownerid"]).Name);

            // SECOND RUN
            writeToCrmConfig.ImportMode = ImportMode.AllChangedValuesOnly;
            writeToCrmConfig.SetOwnerMode = ImportMode.AllChangedValuesOnly;
            contactDatastore.SetValue(0, 2, "Baden");
            contactDatastore.SetValue(0, 7, "John");
            contactDatastore.SetValue(0, 8, "test");
            contactDatastore.SetValue(3, 7, "John");
            contactDatastore.SetValue(3, 8, "test");
            contactDatastore.AddData(new object[] { "Thomas", "Meyer", "New York", 1004, accountName1, "Active", new DateTime(1973, 04, 24), "James", "Test" });

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), contactDatastore, Test_Helpers.ReportProgressMethod);
            
            var contactsId1004 = Crm2013Wrapper.RetrieveMultiple(organizationService, "contact", new ColumnSet(true), new ConditionExpression("new_id", ConditionOperator.Equal, "1004"));
            Assert.AreEqual(1, contactsId1004.Count);

            contactsId1001 = Crm2013Wrapper.RetrieveMultiple(organizationService, "contact", new ColumnSet(true), new ConditionExpression("new_id", ConditionOperator.Equal, "1001"));
            Assert.AreEqual(1, contactsId1001.Count);
            contact1001 = contactsId1001.First();
            Assert.AreEqual("John test", ((EntityReference)contact1001["ownerid"]).Name);

            organizationService.Delete("account", account1);
        }

        private static WriteToDynamicsCrmConfiguration GetContactImportConfiguration()
        {
            var writeToCrmConfig = new WriteToDynamicsCrmConfiguration()
            {
                EntityName = "contact",
                ImportMode = ImportMode.All,
                MultipleFoundMode = MultipleFoundMode.All,
                SetOwnerMode = ImportMode.All,
                SetStateMode = ImportMode.All,
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

            writeToCrmConfig.RelationMapping.Add(new Module.WriteToDynamicsCrm.SDK.RelationMapping()
            {
                EntityName = "systemuser",
                LogicalName = "ownerid",
                Mapping = new List<DataMappingControl.DataMapping>() { 
                    new DataMappingControl.DataMapping() { Source ="OwnerFirstname", Target = "firstname" },
                    new DataMappingControl.DataMapping() { Source ="OwnerLastname", Target = "lastname" }
                }
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
            dataObject.AddColumn(new ColumnMetadata("OwnerFirstname"));
            dataObject.AddColumn(new ColumnMetadata("OwnerLastname"));

            dataObject.AddData(new object[] { "Peter", "Widmer", "Wettingen", 1001, accountName1, "Active", new DateTime(1980, 06, 23), "James", "Test" });
            dataObject.AddData(new object[] { "Joachim 2", "Suter", "Dättwil", 1002, accountName1, "Inactive", new DateTime(2004, 12, 03), "John", "test" });
            dataObject.AddData(new object[] { "James", "Brown", "London", 1003, null, "Active", null, null, null });
            // Doublekey to test it works too
            dataObject.AddData(new object[] { "Peter", "Widmer", "Wettingen", 1001, accountName1, "Active", new DateTime(1980, 06, 23), "James", "Test" });
            dataObject.AddData(new object[] { "Empty", "City", "", 1004, null, "Active", null, null, null });

            return dataObject;
        }

        [TestMethod]
        public void Test_CaseImport()
        {
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

        [TestMethod]
        public void Test_EntityUpdateHandler_SetOwnerMode()
        {
            Guid testGuid = Guid.NewGuid();
            var ownerInSourceWithTestGuid = new EntityReference("systemuser", testGuid);
            var ownerInCrmWithTestGuid = new EntityReference("systemuser", testGuid);
            var ownerInSourceWithRandomGuid = new EntityReference("systemuser", Guid.NewGuid());
            var ownerInCrmWithRandomGuid = new EntityReference("systemuser", Guid.NewGuid());

            bool test1 = EntityUpdateHandler.OwnerMustBeSet(null, ownerInCrmWithRandomGuid, ImportMode.AllChangedValuesOnly);
            Assert.IsFalse(test1);

            bool test2 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithTestGuid, ownerInCrmWithTestGuid, ImportMode.Create);
            Assert.IsFalse(test2);

            bool test3 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithTestGuid, ownerInCrmWithTestGuid, ImportMode.UpdateChangedValuesOnly);
            Assert.IsFalse(test3);

            bool test4 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithTestGuid, ownerInCrmWithTestGuid, ImportMode.AllChangedValuesOnly);
            Assert.IsFalse(test4);

            bool test5 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, ownerInCrmWithTestGuid, ImportMode.UpdateChangedValuesOnly);
            Assert.IsTrue(test5);

            bool test6 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, ownerInCrmWithTestGuid, ImportMode.AllChangedValuesOnly);
            Assert.IsTrue(test6);

            bool test7 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithTestGuid, ownerInCrmWithTestGuid, ImportMode.Update);
            Assert.IsTrue(test7);

            bool test8 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithTestGuid, ownerInCrmWithTestGuid, ImportMode.All);
            Assert.IsTrue(test8);

            bool test9 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, ownerInCrmWithTestGuid, ImportMode.Update);
            Assert.IsTrue(test9);

            bool test10 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, ownerInCrmWithTestGuid, ImportMode.All);
            Assert.IsTrue(test10);

            bool test11 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, null, ImportMode.UpdateChangedValuesOnly);
            Assert.IsTrue(test11);

            bool test12 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, null, ImportMode.AllChangedValuesOnly);
            Assert.IsTrue(test12);

            bool test13 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, null, ImportMode.Update);
            Assert.IsTrue(test13);

            bool test14 = EntityUpdateHandler.OwnerMustBeSet(ownerInSourceWithRandomGuid, null, ImportMode.All);
            Assert.IsTrue(test14);
        }

        [TestMethod]
        public void Test_EntityUpdateHandler_SetStateMode()
        {
            var value3 = new OptionSetValue(3);
            var value4 = new OptionSetValue(4);

            bool test1 = EntityUpdateHandler.StatusMustBeSet(null, value3, ImportMode.All);
            Assert.IsFalse(test1);

            bool test2 = EntityUpdateHandler.StatusMustBeSet(value3, value3, ImportMode.Create);
            Assert.IsFalse(test2);

            bool test3 = EntityUpdateHandler.StatusMustBeSet(value3, value3, ImportMode.UpdateChangedValuesOnly);
            Assert.IsFalse(test3);

            bool test4 = EntityUpdateHandler.StatusMustBeSet(value3, value3, ImportMode.AllChangedValuesOnly);
            Assert.IsFalse(test4);

            bool test5 = EntityUpdateHandler.StatusMustBeSet(value4, value3, ImportMode.UpdateChangedValuesOnly);
            Assert.IsTrue(test5);

            bool test6 = EntityUpdateHandler.StatusMustBeSet(value4, value3, ImportMode.AllChangedValuesOnly);
            Assert.IsTrue(test6);

            bool test7 = EntityUpdateHandler.StatusMustBeSet(value3, value3, ImportMode.Update);
            Assert.IsTrue(test7);

            bool test8 = EntityUpdateHandler.StatusMustBeSet(value3, value3, ImportMode.All);
            Assert.IsTrue(test8);

            bool test9 = EntityUpdateHandler.StatusMustBeSet(value4, value3, ImportMode.Update);
            Assert.IsTrue(test9);

            bool test10 = EntityUpdateHandler.StatusMustBeSet(value4, value3, ImportMode.All);
            Assert.IsTrue(test10);

            bool test11 = EntityUpdateHandler.StatusMustBeSet(value4, null, ImportMode.UpdateChangedValuesOnly);
            Assert.IsTrue(test11);

            bool test12 = EntityUpdateHandler.StatusMustBeSet(value4, null, ImportMode.AllChangedValuesOnly);
            Assert.IsTrue(test12);

            bool test13 = EntityUpdateHandler.StatusMustBeSet(value4, null, ImportMode.Update);
            Assert.IsTrue(test13);

            bool test14 = EntityUpdateHandler.StatusMustBeSet(value4, null, ImportMode.All);
            Assert.IsTrue(test14);
        }
    }
}
