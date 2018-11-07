using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public interface IDataSource
    {
        void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress, bool mappingPreview);
    }
}
