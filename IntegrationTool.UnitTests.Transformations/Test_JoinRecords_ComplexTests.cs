using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Transformations
{
    [TestClass]
    public class Test_JoinRecords_ComplexTests
    {
        [TestMethod]
        public void ComplexInnerJoinTest()
        {
            // TODO Write Complex Join Test
        }

        public IDatastore GetDatastoreA()
        {
            IDatastore store = DataStoreFactory.GetDatastore();
            store.AddColumn(new ColumnMetadata("TableAColA"));
            store.AddColumn(new ColumnMetadata("TableAColB"));
            store.AddColumn(new ColumnMetadata("TableAFull"));

            store.AddData(new object[] { "A", "B", "A_B" });
            store.AddData(new object[] { "A", null, "A_Null" });
            store.AddData(new object[] { null, null, "Null_Null" });

            return store;

        }
    }
}
