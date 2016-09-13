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

            AnalyzeDatastores(datastore1, datastore2);
            BuildDatastoreKeys();

            IDatastoreColumnHashBuilder smallDataStoreHashBuilder = new DatastoreColumnHashBuilder(smallDatastore, smallDatastoreKeys);

            int [] largeDatastoreColumnIndexes = GetColumnIndexes();
            for (int i = 0; i < largeDatastore.Count; i++)
            {
                int rowHash = smallDataStoreHashBuilder.GetRowHash(largeDatastore[i], largeDatastoreColumnIndexes);
                object[] smallDatastoreRow = smallDataStoreHashBuilder.GetRowByHash(rowHash);
                if(smallDatastoreRow != null)
                {
                    // TODO Implement the resultset FoundRow
                }
            }
            return resultDatastore;
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
