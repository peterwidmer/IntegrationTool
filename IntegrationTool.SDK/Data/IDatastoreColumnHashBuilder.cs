using System;
namespace IntegrationTool.SDK.Data
{
    public interface IDatastoreColumnHashBuilder
    {
        void BuildHashes();
        object[] GetRowByHash(int hashcode);
        int GetRowHash(object[] row, int[] columnIndexes);
    }
}
