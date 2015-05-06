using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.SDK
{
    public interface IModule
    {
        void SetConfiguration(ConfigurationBase configurationBase);
        UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject);
    }
}
