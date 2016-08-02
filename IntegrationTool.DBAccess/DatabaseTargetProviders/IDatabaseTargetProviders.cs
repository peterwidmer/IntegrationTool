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
        List<object[]> ResolveRecordInDatabase(DbRecord dbRecord);
        void CreateRecordInDatabase(DbRecord dbRecord);
        void UpdateRecordInDatabase(DbRecord dbRecord);
    }
}
