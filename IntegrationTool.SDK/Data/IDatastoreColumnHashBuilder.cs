using System;
using System.Collections.Generic;
namespace IntegrationTool.SDK.Data
{
    public interface IDatastoreColumnHashBuilder
    {
        void BuildHashes();
        IEnumerable<object[]> GetRowsByHash(int hashcode);
        RowHash GetRowHash(object[] row, int[] columnIndexes);
    }
}
