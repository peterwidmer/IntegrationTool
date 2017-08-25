using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.WriteToDynamicsCrm;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK;
using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Targets
{
    [TestClass]
    public class Test_WriteToDynamicsCrm_LargeDataset
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
        public void Test_DynamicsCrm_ImportLargeDataset()
        {
            var writeToCrmConfig = GetAccountImportConfiguration();

            IDatastore accountDatastore = Test_Helpers.LoadExcelsheet(@"TestData\LargeDataset.xlsx", "Accounts");
            IModule module = Activator.CreateInstance(typeof(WriteToDynamicsCrm)) as IModule;
            module.SetConfiguration(writeToCrmConfig);

            ((IDataTarget)module).WriteData(connection, new DummyDatabaseInterface(), accountDatastore, Test_Helpers.ReportProgressMethod);
        }

        private static WriteToDynamicsCrmConfiguration GetAccountImportConfiguration()
        {
            var writeToCrmConfig = new WriteToDynamicsCrmConfiguration()
            {
                EntityName = "account",
                ImportMode = ImportMode.All,
                MultipleFoundMode = MultipleFoundMode.All,
                SetOwnerMode = ImportMode.All,
                SetStateMode = ImportMode.All,
                ConfigurationId = Guid.NewGuid(),
                SelectedConnectionConfigurationId = Test_Helpers.CRMCONNECTIONID
            };

            writeToCrmConfig.PrimaryKeyAttributes.Add("accountnumber");
            writeToCrmConfig.Mapping.Add(new DataMapping("AccountId", "accountnumber"));
            writeToCrmConfig.Mapping.Add(new DataMapping("Account Name", "name"));
            writeToCrmConfig.Mapping.Add(new DataMapping("Account Type", "customertypecode"));
            
            writeToCrmConfig.PicklistMapping.Add(new PicklistMapping()
            {
                LogicalName = "customertypecode",
                MappingType = PicklistMappingType.Manual,
                Mapping = new List<DataMappingControl.DataMapping>() 
                { 
                    new DataMappingControl.DataMapping("Customer Type", "3"),
                    new DataMappingControl.DataMapping("Competitor Type", "1"),
                    new DataMappingControl.DataMapping("Partner Type", "5")
                }
            });
            return writeToCrmConfig;
        }
    }
}
