using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.Module.JoinRecords;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using IntegrationTool.UnitTests.Transformations.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            var datastoreA = GetDatastoreA();
            var datastoreB = GetDatastoreB();

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration(JoinRecordsJoinType.InnerJoin) };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), datastoreA, datastoreB, Test_Helpers.ReportProgressMethod);

            AssertComplexInnerJoin(resultDatastore);
        }

        private void AssertComplexInnerJoin(IDatastore resultDatastore)
        {
            var joinRecordsRows = ConvertToJoinRecordsRows(resultDatastore);

            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "A_B" && row.TableBFull == "A_B") == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "A_B" && row.TableBFull == "A_B_2") == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "A_Null" && row.TableBFull == "A_Null") == 0);
        }

        [TestMethod]
        public void ComplexLeftJoinTest()
        {
            var datastoreA = GetDatastoreA();
            var datastoreB = GetDatastoreB();

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration(JoinRecordsJoinType.LeftJoin) };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), datastoreA, datastoreB, Test_Helpers.ReportProgressMethod);

            AssertComplexLeftJoin(resultDatastore);
        }

        private void AssertComplexLeftJoin(IDatastore resultDatastore)
        {
            var joinRecordsRows = ConvertToJoinRecordsRows(resultDatastore);

            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "A_B" && row.TableBFull == "A_B") == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "A_B" && row.TableBFull == "A_B_2") == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "A_Null" && row.TableBFull == null) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.TableAFull == "Null_Null" && row.TableBFull == null) == 1);
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

        public IDatastore GetDatastoreB()
        {
            IDatastore store = DataStoreFactory.GetDatastore();
            store.AddColumn(new ColumnMetadata("TableBColA"));
            store.AddColumn(new ColumnMetadata("TableBColB"));
            store.AddColumn(new ColumnMetadata("TableBFull"));

            store.AddData(new object[] { "A", "B", "A_B" });
            store.AddData(new object[] { "A", null, "A_Null" });
            store.AddData(new object[] { "A", "B", "A_B_2" });

            return store;
        }

        private JoinRecordsConfiguration GetJoinRecordsConfiguration(JoinRecordsJoinType joinType)
        {
            return new JoinRecordsConfiguration()
            {
                JoinType = joinType,
                JoinMapping = new List<DataMapping>()
                {
                    new DataMapping("TableAColA", "TableBColA"),
                    new DataMapping("TableAColB", "TableBColB")
                },
                OutputColumns = new ObservableCollection<OutputColumn>()
                {
                    new OutputColumn() { Column = new ColumnMetadata("TableAFull"), DataStream= DataStreamSource.Left },
                    new OutputColumn() { Column = new ColumnMetadata("TableBFull"), DataStream= DataStreamSource.Right }
                }
            };
        }

        public List<JoinRecordsRowComplexTest> ConvertToJoinRecordsRows(IDatastore datastore)
        {
            List<JoinRecordsRowComplexTest> joinRecordsRows = new List<JoinRecordsRowComplexTest>();
            for (int i = 0; i < datastore.Count; i++)
            {
                JoinRecordsRowComplexTest row = new JoinRecordsRowComplexTest();
                row.TableAFull = (string)datastore[i][datastore.Metadata["TableAFull"].ColumnIndex];
                row.TableBFull = (string)datastore[i][datastore.Metadata["TableBFull"].ColumnIndex];

                joinRecordsRows.Add(row);
            }
            return joinRecordsRows;
        }


    }
}
