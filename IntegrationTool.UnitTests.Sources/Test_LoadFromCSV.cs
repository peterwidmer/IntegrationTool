using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.Module.ConnectToTextFile;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.Module.LoadFromCSV;

namespace IntegrationTool.UnitTests.Sources
{
    [TestClass]
    public class Test_LoadFromCSV
    {
        [TestMethod]
        public void GeneralLoadTestCSV()
        {
            ConnectToTextFileConfiguration configuration = new ConnectToTextFileConfiguration();
            configuration.FilePath = @"TestData\LoadFromCSV\TestCSV.csv";

            IConnection csvConnection = new ConnectToTextFile() { Configuration = configuration };

            IDataSource loadFromCSV = new LoadFromCSV();

            IDatastore dataObject = new DataObject();
            loadFromCSV.LoadData(csvConnection, dataObject, ReportProgressMethod);

            var firstObject = dataObject[0];
            Assert.AreEqual<string>(firstObject[0].ToString(), "1");
            Assert.AreEqual<string>(firstObject[6].ToString(), "9/19/2015");
            
            var lastObject = dataObject[999];
            Assert.AreEqual<string>(lastObject[0].ToString(), "1000");
            Assert.AreEqual<string>(lastObject[6].ToString(), "4/5/2015");
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
        }
    }
}
