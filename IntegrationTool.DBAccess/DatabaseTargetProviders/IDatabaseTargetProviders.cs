using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess.DatabaseTargetProviders
{
    public interface IDatabaseTargetProvider
    {
        void ResolveRecordInDatabase(string tableName, KeyValuePair<string, object> [] recordIdentifiers);
    }
}
