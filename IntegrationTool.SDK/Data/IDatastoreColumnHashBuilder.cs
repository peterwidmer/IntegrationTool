using System;
using System.Collections.Generic;
namespace IntegrationTool.SDK.Data
{
    public interface IDatastoreColumnHashBuilder
    {
        void BuildHashes();
        IEnumerable<object[]> GetRowsByHash(int hashcode);
        int GetRowHash(object[] row, int[] columnIndexes);
    }
}
