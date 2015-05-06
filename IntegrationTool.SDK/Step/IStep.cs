using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Step
{
    public interface IStep
    {
        void Execute(IConnection connection, IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress);
    }
}
