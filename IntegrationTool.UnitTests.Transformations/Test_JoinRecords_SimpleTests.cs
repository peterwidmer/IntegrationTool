using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK;
using IntegrationTool.Module.JoinRecords;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;
using IntegrationTool.UnitTests.Transformations.Classes;

namespace IntegrationTool.UnitTests.Transformations
{
    [TestClass]
    public class Test_JoinRecords_SimpleTests
    {
        [TestMethod]
        public void SimpleInnerJoin()
        {
            var companyDatastore = GetCompanyDatastore();
            var personDatastore = GetPersonDatastore();

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration(JoinRecordsJoinType.InnerJoin) };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), companyDatastore, personDatastore, Test_Helpers.ReportProgressMethod);

            AssertSimpleInnerJoin(resultDatastore);
        }

        [TestMethod]
        public void SimpleInnerJoinSwitchedLargeSmallDatastore()
        {
            var companyDatastore = GetLargerCompanyStore();
            var personDatastore = GetPersonDatastore();

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration(JoinRecordsJoinType.InnerJoin) };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), companyDatastore, personDatastore, Test_Helpers.ReportProgressMethod);

            AssertSimpleInnerJoin(resultDatastore);            
        }

        public void AssertSimpleInnerJoin(IDatastore resultDatastore)
        {
            List<JoinRecordsRow> joinRecordsRows = ConvertToJoinRecordsRows(resultDatastore);

            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 1 && row.PersonId == 2) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 2 && row.PersonId == 3) == 1);
        }

        [TestMethod]
        public void SimpleLeftJoin()
        {
            var companyDatastore = GetLargerCompanyStore();
            var personDatastore = GetPersonDatastore();

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration(JoinRecordsJoinType.LeftJoin) };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), companyDatastore, personDatastore, Test_Helpers.ReportProgressMethod);

            List<JoinRecordsRow> joinRecordsRows = ConvertToJoinRecordsRows(resultDatastore);

            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 1 && row.PersonId == 2) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 2 && row.PersonId == 3) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 3 && row.PersonId == null) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 4 && row.PersonId == null) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 5 && row.PersonId == null) == 1);
            Assert.IsTrue(joinRecordsRows.Count(row => row.CompanyId == 6 && row.PersonId == null) == 1);
        }

        

        private JoinRecordsConfiguration GetJoinRecordsConfiguration(JoinRecordsJoinType joinType)
        {
            return new JoinRecordsConfiguration()
            {
                JoinType = joinType,
                JoinMapping = new List<DataMapping>()
                {
                    new DataMapping("ContactPerson", "PersonId")
                },
                OutputColumns = new ObservableCollection<OutputColumn>()
                {
                    new OutputColumn() { Column = new ColumnMetadata("CompanyId"), DataStream= DataStreamSource.Left },
                    new OutputColumn() { Column = new ColumnMetadata("CompanyName"), DataStream= DataStreamSource.Left},
                    new OutputColumn() { Column = new ColumnMetadata("PersonId"), DataStream= DataStreamSource.Right},
                    new OutputColumn() { Column = new ColumnMetadata("Firstname"), DataStream= DataStreamSource.Right},
                    new OutputColumn() { Column = new ColumnMetadata("Lastname"), DataStream= DataStreamSource.Right}
                }
            };
        }

        private IDatastore GetLargerCompanyStore()
        {
            IDatastore store = GetCompanyDatastore();

            store.AddData(new object[] { 3, "comp 3", 8 });
            store.AddData(new object[] { 4, "comp 4", 9 });
            store.AddData(new object[] { 5, "comp 5", 10 });
            store.AddData(new object[] { 6, "comp 6", 11 });

            return store;
        }

        private IDatastore GetCompanyDatastore()
        {
            IDatastore store = DataStoreFactory.GetDatastore();
            store.AddColumn(new ColumnMetadata("CompanyId"));
            store.AddColumn(new ColumnMetadata("CompanyName"));
            store.AddColumn(new ColumnMetadata("ContactPerson"));

            store.AddData(new object [] { 1, "comp 1", 2 });
            store.AddData(new object[] { 2, "comp 2", 3 });

            return store;
        }

        private IDatastore GetPersonDatastore()
        {
            IDatastore store = DataStoreFactory.GetDatastore();
            store.AddColumn(new ColumnMetadata("PersonId"));
            store.AddColumn(new ColumnMetadata("Firstname"));
            store.AddColumn(new ColumnMetadata("Lastname"));

            store.AddData(new object[] { 1, "Simon", "Miller" });
            store.AddData(new object[] { 2, "Frank", "Houston" });
            store.AddData(new object[] { 3, "Jill", "Smith" });
            store.AddData(new object[] { 4, "Gregory", "Thomson" });

            return store;
        }

        public List<JoinRecordsRow> ConvertToJoinRecordsRows(IDatastore datastore)
        {
            List<JoinRecordsRow> joinRecordsRows = new List<JoinRecordsRow>();
            for(int i=0; i < datastore.Count; i++)
            {
                JoinRecordsRow row = new JoinRecordsRow();
                row.CompanyId = (int ?)datastore[i][datastore.Metadata["CompanyId"].ColumnIndex];
                row.CompanyName = (string)datastore[i][datastore.Metadata["CompanyName"].ColumnIndex];
                row.PersonId = (int?)datastore[i][datastore.Metadata["PersonId"].ColumnIndex];
                row.Firstname = (string)datastore[i][datastore.Metadata["Firstname"].ColumnIndex];
                row.Lastname = (string)datastore[i][datastore.Metadata["Lastname"].ColumnIndex];

                joinRecordsRows.Add(row);
            }
            return joinRecordsRows;
        }
    }
}
