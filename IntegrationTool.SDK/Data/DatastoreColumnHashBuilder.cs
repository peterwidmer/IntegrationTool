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
        private Dictionary<int, int> rowHahes = new Dictionary<int, int>();

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
                rowHahes.Add(rowHash, i);
            }
        }

        public int GetRowHash(object [] row, int [] columnIndexes)
        {
            object[] indexObjects = new object[columnIndexes.Length];
            for (var i2 = 0; i2 < columnIndexes.Length; i2++)
            {
                indexObjects[i2] = row[i2];
            }

            return indexObjects.GetHashCode();
        }

        public object [] GetRowByHash(int hashcode)
        {
            int rowIndex;
            if(rowHahes.TryGetValue(hashcode, out rowIndex))
            {
                return datastore[rowIndex];
            }
            else
            {
                return null;
            }
        }
    }
}
