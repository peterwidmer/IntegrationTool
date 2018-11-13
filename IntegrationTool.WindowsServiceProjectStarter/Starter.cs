using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using IntegrationTool.WindowsServiceProjectStarter;

namespace Geberit.Crm.WorkflowEngine.Service
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MigrationService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}