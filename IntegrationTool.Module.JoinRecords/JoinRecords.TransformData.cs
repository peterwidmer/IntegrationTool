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

        public IDatastore TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore datastore1, IDatastore datastore2, ReportProgressMethod reportProgress)
        {
            IDatastore resultDatastore = DataStoreFactory.GetDatastore();

            AnalyzeDatastores(datastore1, datastore2);

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

    }
}
