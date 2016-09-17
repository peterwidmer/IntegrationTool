using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK;

namespace IntegrationTool.UnitTests.Transformations
{
    [TestClass]
    public class Test_JoinRecords
    {
        [TestMethod]
        public void TestMethod1()
        {
            
        }

        private IDatastore GetCompanyDatastore()
        {
            IDatastore store = DataStoreFactory.GetDatastore();
            store.AddColumn(new ColumnMetadata("CompanyId"));
            store.AddColumn(new ColumnMetadata("CompanyName"));
            
            store.AddData(new object [] { 1, "comp 1" });
            store.AddData(new object[] { 2, "comp 2" });

            return store;
        }
    }
}
