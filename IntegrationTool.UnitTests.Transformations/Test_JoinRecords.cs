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
            var leftDatastore = GetCompanyDatastore();
            var rightDatastore = GetPersonDatastore();

            var joinRecordsConfiguration = new JoinRecordsConfiguration()
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
                    new OutputColumn() { Column = new ColumnMetadata("FirstName"), DataStream= DataStreamSource.Right},
                    new OutputColumn() { Column = new ColumnMetadata("LastName"), DataStream= DataStreamSource.Right}
                }
            };

            var joinRecords = new JoinRecords() { Configuration = joinRecordsConfiguration };
            joinRecords.TransformData(null, new DummyDatabaseInterface(), leftDatastore, rightDatastore, Test_Helpers.ReportProgressMethod);
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
