using IntegrationTool.DBAccess;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.Module.WriteToDynamicsCrm;
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
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            IOrganizationService service = new OrganizationService(crmConnection);
            
            string accountName1 = Guid.NewGuid().ToString();
            Entity account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = service.Create(account);

            IntegrationTool.Module.WriteToDynamicsCrm.WriteToDynamicsCrmConfiguration writeToCrmConfig = new IntegrationTool.Module.WriteToDynamicsCrm.WriteToDynamicsCrmConfiguration();
            writeToCrmConfig.EntityName = "contact";
            writeToCrmConfig.PrimaryKeyAttributes.Add("new_id");
            writeToCrmConfig.ImportMode = Module.WriteToDynamicsCrm.SDK.Enums.ImportMode.All;
            writeToCrmConfig.MultipleFoundMode = Module.WriteToDynamicsCrm.SDK.Enums.MultipleFoundMode.All;
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "ID", Target = "new_id" });
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "FirstName", Target = "firstname" });
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "LastName", Target = "lastname" });
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "Status", Target = "statuscode" });
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "Birthdate", Target = "birthdate" });
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
            writeToCrmConfig.ConfigurationId = Guid.NewGuid();
            writeToCrmConfig.SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID;
            writeToCrmConfig.PicklistMapping.Add(new Module.WriteToDynamicsCrm.SDK.PicklistMapping()
            {
                LogicalName = "statuscode",
                MappingType = Module.WriteToDynamicsCrm.SDK.Enums.PicklistMappingType.Manual,
                Mapping = new List<DataMappingControl.DataMapping>() 
                { 
                    new DataMappingControl.DataMapping() 
                    {
                        Source = "Active", Target = "1"
                    },
                    new DataMappingControl.DataMapping() 
                    {
                        Source = "Inactive", Target = "2"
                    }
                }
            });
            IDatastore dataObject = new IntegrationTool.SDK.DataObject();
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

            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrm)) as IModule;
            module.SetConfiguration(writeToCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), dataObject, Test_Helpers.ReportProgressMethod);

            service.Delete("account", account1);
        }

        [TestMethod]
        public void Test_CaseImport()
        {
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            IOrganizationService service = new OrganizationService(crmConnection);

            string accountName1 = Guid.NewGuid().ToString();
            Entity account = new Entity("account");
            account.Attributes.Add("name", accountName1);
            Guid account1 = service.Create(account);

            IntegrationTool.Module.WriteToDynamicsCrm.WriteToDynamicsCrmConfiguration writeToCrmConfig = new IntegrationTool.Module.WriteToDynamicsCrm.WriteToDynamicsCrmConfiguration();
            writeToCrmConfig.EntityName = "incident";
            writeToCrmConfig.PrimaryKeyAttributes.Add("ticketnumber");
            writeToCrmConfig.ImportMode = Module.WriteToDynamicsCrm.SDK.Enums.ImportMode.All;
            writeToCrmConfig.MultipleFoundMode = Module.WriteToDynamicsCrm.SDK.Enums.MultipleFoundMode.All;
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "CaseID", Target = "ticketnumber" });
            writeToCrmConfig.Mapping.Add(new IntegrationTool.DataMappingControl.DataMapping() { Source = "CaseTitle", Target = "title" });

            writeToCrmConfig.RelationMapping.Add(new Module.WriteToDynamicsCrm.SDK.RelationMapping()
            {
                EntityName = "account",
                LogicalName = "customerid",
                Mapping = new List<DataMappingControl.DataMapping>() { new DataMappingControl.DataMapping()
                    {
                        Source ="CompanyName",
                        Target = "name"
                    } }
            });
            writeToCrmConfig.ConfigurationId = Guid.NewGuid();
            writeToCrmConfig.SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID;
            
            IDatastore dataObject = new IntegrationTool.SDK.DataObject();
            dataObject.AddColumn(new ColumnMetadata("CaseID"));
            dataObject.AddColumn(new ColumnMetadata("CaseTitle"));
            dataObject.AddColumn(new ColumnMetadata("CompanyName"));


            dataObject.AddData(new object[] { "CA-100", "101 Title", accountName1 });
            dataObject.AddData(new object[] { "CA-101", "Anöther Title", null });
            dataObject.AddData(new object[] { "CA-102", "A'Title\"chen", accountName1 });

            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrm)) as IModule;
            module.SetConfiguration(writeToCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), dataObject, Test_Helpers.ReportProgressMethod);

            service.Delete("account", account1);
        }
    }
}
