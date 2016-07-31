using IntegrationTool.DataMappingControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess.DatabaseTargetProviders
{
    public interface IDatabaseTargetProvider
    {
        List<object[]> ResolveRecordInDatabase(string tableName, KeyValuePair<string, object>[] recordIdentifiers);
        void CreateRecordInDatabase(DbRecord dbRecord);
        void UpdateRecordInDatabase(DbRecord dbRecord, KeyValuePair<string, object>[] recordIdentifiers);
    }
}
