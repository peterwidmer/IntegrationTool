using IntegrationTool.Module.ConnectToSharepoint;
using IntegrationTool.Module.LoadFromDynamicsCrm;
using IntegrationTool.Module.LoadFromSharepoint;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Sources.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Sources
{
    [TestClass]
    public class Test_LoadFromSharepoint
    {
        private static Guid CRMCONNECTIONID = new Guid("4D3F2E27-71EC-4631-8E98-5915E99FCED2");
        private static IConnection connection;

        [ClassInitialize]
        public static void InitializeCrm2013Wrapper(TestContext context)
        {
            var configuration = new ConnectToSharepointConfiguration()
            {
                AuthenticationType = SharepointAuthenticationType.SharepointOnline,
                User = Settings.Default.SharepointUsername,
                SiteUrl = "https://mysptest79.sharepoint.com/sites/TestWebsite",
                Password = File.ReadAllText(@"C:\Temp\IntegrationToolTests\SharepointPassword.txt")
            };

            configuration.AuthenticationType = SharepointAuthenticationType.SharepointOnline;

            connection = new ConnectToSharepoint() { Configuration = configuration };
        }

        [TestMethod]
        public void GeneralLoadTestSharepoint()
        {
            var configuration = new LoadFromSharepointConfiguration()
            {
                ListName = "My Test List",
                CamlQueryViewXml = "<View><Query><Where><Geq><FieldRef Name='ID'/><Value Type='Number'>1</Value></Geq></Where></Query><RowLimit>100</RowLimit></View>"
            };

            IDataSource loadFromDynamicsCrm = new LoadFromSharepoint() { Configuration = configuration };

            IDatastore dataObject = DataStoreFactory.GetDatastore();
            loadFromDynamicsCrm.LoadData(connection, dataObject, ReportProgressMethod, false);
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
        }
    }
}
