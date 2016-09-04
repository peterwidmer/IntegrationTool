using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.SDK
{
    public interface IDataMerge
    {
        void TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore datastore1, IDatastore datastore2, ReportProgressMethod reportProgress);
        UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore datastoreOne, IDatastore datastoreTwo);
    }
}
