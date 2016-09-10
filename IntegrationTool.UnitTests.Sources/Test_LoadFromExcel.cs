using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using IntegrationTool.Module.LoadFromExcel;
using IntegrationTool.SDK;
using System.Collections.Generic;
using IntegrationTool.Module.ConnectToExcel;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Data;

namespace IntegrationTool.UnitTests.Sources
{
    [TestClass]
    public class Test_LoadFromExcel
    {
        [TestMethod]
        public void GeneralLoadTestExcel()
        {
            ConnectToExcelConfiguration configuration = new ConnectToExcelConfiguration();
            configuration.FilePath = @"TestData\LoadFromExcel\TestData.xlsx";
            configuration.SheetName = "Contacts";

            IConnection excelConnection = new ConnectToExcel() { Configuration = configuration };
            
            IDataSource loadFromExcel = new LoadFromExcel();

            IDatastore dataObject = DataStoreFactory.GetDatastore();
            loadFromExcel.LoadData(excelConnection, dataObject, ReportProgressMethod);
        }

        private void ReportProgressMethod(SimpleProgressReport progress)
        {
        }
    }
}
