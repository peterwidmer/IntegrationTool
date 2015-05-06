using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.SDK.Logging
{
    public interface ILogRendering
    {
        UserControl RenderLogWindow(IDatabaseInterface databaseInterface);
    }
}
