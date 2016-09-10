using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data
{
    public static class DataStoreFactory
    {
        public static IDatastore GetDatastore()
        {
            return new DataObject();
        }
    }
}
