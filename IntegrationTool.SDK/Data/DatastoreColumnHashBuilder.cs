using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data
{
    public class DatastoreColumnHashBuilder : IDatastoreColumnHashBuilder
    {
        private IDatastore datastore;
        private List<string> columns;
        private int [] columnIndexes;
        private Dictionary<int, List<int>> rowHashes = new Dictionary<int, List<int>>();

        public DatastoreColumnHashBuilder(IDatastore datastore, List<string> columns)
        {
            this.datastore = datastore;
            this.columns = columns;
            columns.Sort();
            columnIndexes = new int[columns.Count];
            for(int i=0; i < columns.Count; i++)
            {
                columnIndexes[i] = datastore.Metadata[columns[i]].ColumnIndex;
            }
        }

        public void BuildHashes()
        {
            for (int i = 0; i < datastore.Count; i++)
            {
                int rowHash = GetRowHash(datastore[i], columnIndexes);
                if (rowHashes.ContainsKey(rowHash))
                {
                    rowHashes[rowHash].Add(i);
                }
                else
                {
                    rowHashes.Add(rowHash, new List<int>() { i });
                }
            }
        }

        public int GetRowHash(object [] row, int [] columnIndexes)
        {
            // TODO Write correct hashbuilder
            object[] indexObjects = new object[columnIndexes.Length];
            for (var i = 0; i < columnIndexes.Length; i++)
            {
                indexObjects[i] = row[columnIndexes[i]];
            }

            return indexObjects.GetHashCode();
        }

        public IEnumerable<object []> GetRowsByHash(int hashcode)
        {
            List<int> rowIndexes;
            if (rowHashes.TryGetValue(hashcode, out rowIndexes))
            {
                foreach(var rowIndex in rowIndexes)
                {
                    yield return datastore[rowIndex];
                }
            }
        }
    }
}
