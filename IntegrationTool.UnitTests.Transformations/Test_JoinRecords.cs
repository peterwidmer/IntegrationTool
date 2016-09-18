using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK;
using IntegrationTool.Module.JoinRecords;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using IntegrationTool.DataMappingControl;
using IntegrationTool.DBAccess;

namespace IntegrationTool.UnitTests.Transformations
{
    [TestClass]
    public class Test_JoinRecords
    {
        [TestMethod]
        public void SimpleInnerJoin()
        {
            var companyDatastore = GetCompanyDatastore();
            var personDatastore = GetPersonDatastore();

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration() };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), companyDatastore, personDatastore, Test_Helpers.ReportProgressMethod);
            Assert.IsTrue(resultDatastore.Count == 2);
        }

        [TestMethod]
        public void SimpleInnerJoinSwitchedLargeSmallDatastore()
        {
            var companyDatastore = GetCompanyDatastore();
            var personDatastore = GetPersonDatastore();
            
            IncreaseCompanyStoreToBeLargerThanPersonStore(companyDatastore);

            var joinRecords = new JoinRecords() { Configuration = GetJoinRecordsConfiguration() };
            var resultDatastore = joinRecords.TransformData(null, new DummyDatabaseInterface(), companyDatastore, personDatastore, Test_Helpers.ReportProgressMethod);
            Assert.IsTrue(resultDatastore.Count == 2);
        }

        private JoinRecordsConfiguration GetJoinRecordsConfiguration()
        {
            return new JoinRecordsConfiguration()
            {
                JoinType = JoinRecordsJoinType.InnerJoin,
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

        private void IncreaseCompanyStoreToBeLargerThanPersonStore(IDatastore store)
        {
            store.AddData(new object[] { 3, "comp 3", 8 });
            store.AddData(new object[] { 4, "comp 4", 9 });
            store.AddData(new object[] { 5, "comp 5", 10 });
            store.AddData(new object[] { 6, "comp 6", 11 });
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
    }
}
