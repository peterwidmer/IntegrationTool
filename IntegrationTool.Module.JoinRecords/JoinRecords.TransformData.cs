using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.JoinRecords
{
    public partial class JoinRecords
    {
        private IDatastore largeDatastore;
        private IDatastore smallDatastore;
        private List<string> smallDatastoreKeys = new List<string>();
        private List<string> largeDatastoreKeys = new List<string>();

        public IDatastore TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore datastore1, IDatastore datastore2, ReportProgressMethod reportProgress)
        {
            IDatastore resultDatastore = DataStoreFactory.GetDatastore();

            InitializeOutputColumnsConfiguration();
            InitializeResultDatastore(resultDatastore);
            AnalyzeDatastores(datastore1, datastore2);
            BuildDatastoreKeys();
            JoinDatastoreRecords(resultDatastore);

            return resultDatastore;
        }

        public void InitializeOutputColumnsConfiguration()
        {
            for(int i=0; i < this.Configuration.OutputColumns.Count; i++)
            {
                this.Configuration.OutputColumns[i].Column.ColumnIndex = i;
            }
        }

        private void InitializeResultDatastore(IDatastore resultDatastore)
        {
            foreach(var outputColumn in this.Configuration.OutputColumns)
            {
                string columnName = String.IsNullOrEmpty(outputColumn.ColumnAlias) ? outputColumn.Column.ColumnName : outputColumn.ColumnAlias;
                resultDatastore.AddColumn(new ColumnMetadata(columnName));
            }
        }

        private void JoinDatastoreRecords(IDatastore resultDatastore)
        {
            IDatastoreColumnHashBuilder smallDataStoreHashBuilder = new DatastoreColumnHashBuilder(smallDatastore, smallDatastoreKeys);
            smallDataStoreHashBuilder.BuildHashes();

            int[] largeDatastoreColumnIndexes = GetColumnIndexes();
            for (int rowIndex = 0; rowIndex < largeDatastore.Count; rowIndex++)
            {
                int rowHash = smallDataStoreHashBuilder.GetRowHash(largeDatastore[rowIndex], largeDatastoreColumnIndexes);
                var smallDatastoreRows = smallDataStoreHashBuilder.GetRowsByHash(rowHash);

                List<object[]> joinedRows = null;
                switch (this.Configuration.JoinType)
                {
                    case JoinRecordsJoinType.LeftJoin:
                        joinedRows = ExecuteLeftJoin(rowIndex, smallDatastoreRows);
                        break;

                    case JoinRecordsJoinType.InnerJoin:
                        joinedRows = ExecuteInnerJoin(rowIndex, smallDatastoreRows);
                        break;
                }
                
                AddJoinedRowsToDatastore(resultDatastore, joinedRows);
            }
        }

        private void AddJoinedRowsToDatastore(IDatastore resultDatastore, List<object[]> joinedRows)
        {
            foreach(var joinedRow in joinedRows)
            {
                resultDatastore.AddData(joinedRow);
            }
        }

        private List<object[]> ExecuteLeftJoin(int rowIndex, IEnumerable<object[]> smallDatastoreRows)
        {
            var innerJoinResult = ExecuteInnerJoin(rowIndex, smallDatastoreRows);
            if(innerJoinResult.Count ==0)
            {
                object []leftJoinRow = new object[this.Configuration.OutputColumns.Count];
                foreach (var outputColumn in this.Configuration.OutputColumns.Where(t => t.DataStream == DataStreamSource.Left))
                {
                    object value = largeDatastore[rowIndex][largeDatastore.Metadata[outputColumn.Column.ColumnName].ColumnIndex];
                    leftJoinRow[outputColumn.Column.ColumnIndex] = value;
                }
                innerJoinResult.Add(leftJoinRow);
            }

            return innerJoinResult;
        }

        private List<object[]> ExecuteInnerJoin(int rowIndex, IEnumerable<object[]> smallDatastoreRows)
        {
            List<object[]> joinedRows = new List<object[]>();
            foreach (var row in smallDatastoreRows)
            {
                object[] resultRow = new object[this.Configuration.OutputColumns.Count];
                foreach (var outputColumn in this.Configuration.OutputColumns)
                {
                    object value = null;
                    if (outputColumn.DataStream == DataStreamSource.Left)
                    {
                        value = largeDatastore[rowIndex][largeDatastore.Metadata[outputColumn.Column.ColumnName].ColumnIndex];
                    }
                    else
                    {
                        value = row[smallDatastore.Metadata[outputColumn.Column.ColumnName].ColumnIndex];
                    }

                    resultRow[outputColumn.Column.ColumnIndex] = value;
                }
                joinedRows.Add(resultRow);
            }

            return joinedRows;
        }

        private void AnalyzeDatastores(IDatastore datastore1, IDatastore datastore2)
        {
            if (datastore1.Count > datastore2.Count)
            {
                largeDatastore = datastore1;
                smallDatastore = datastore2;
            }
            else
            {
                largeDatastore = datastore2;
                smallDatastore = datastore1;
                foreach(var mapping in this.Configuration.JoinMapping)
                {
                    string target = mapping.Target;
                    string source = mapping.Source;
                    mapping.Source = target;
                    mapping.Target = source;
                }
                foreach(var outputColumn in this.Configuration.OutputColumns)
                {
                    outputColumn.DataStream = outputColumn.DataStream == DataStreamSource.Left ? DataStreamSource.Right : DataStreamSource.Left;
                }
            }
        }

        private void BuildDatastoreKeys()
        {
            foreach(var mapping in this.Configuration.JoinMapping)
            {
                largeDatastoreKeys.Add(mapping.Source);
                smallDatastoreKeys.Add(mapping.Target);                
            }
        }

        private int [] GetColumnIndexes()
        {
            int[] columnIndexes = new int[largeDatastoreKeys.Count];
            for (int i = 0; i < largeDatastoreKeys.Count; i++)
            {
                columnIndexes[i] = largeDatastore.Metadata[largeDatastoreKeys[i]].ColumnIndex;
            }

            return columnIndexes;
        }

    }
}
