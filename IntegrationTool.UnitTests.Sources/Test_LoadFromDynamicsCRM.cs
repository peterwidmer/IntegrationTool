using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.Module.ConnectToDynamicsCrm;
using IntegrationTool.SDK;
using IntegrationTool.Module.LoadFromDynamicsCrm;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Sources.Properties;

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

            IDataSource loadFromDynamicsCrm = new LoadFromDynamicsCrm();

            IDatastore dataObject = new DataObject();
            loadFromDynamicsCrm.LoadData(connection, dataObject, ReportProgressMethod);
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
        }
    }
}
