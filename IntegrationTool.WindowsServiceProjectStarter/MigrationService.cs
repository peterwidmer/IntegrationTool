using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.WindowsServiceProjectStarter
{
    public class MigrationService : ServiceBase
    {
        private EventLog _eventLog;
        private MigrationRunner _engine;
        public MigrationService()
        {
            _eventLog = new EventLog();
            if (!EventLog.SourceExists("MigrationService"))
            {
                EventLog.CreateEventSource(
                    "MigrationService", "Migration ServiceLog");
            }
            _eventLog.Source = "MigrationService";
            _eventLog.Log = "Migration ServiceLog";
        }

        protected override void OnStart(string[] args)
        {
            _eventLog.WriteEntry("Try starting service...");
            _engine = new MigrationRunner();
            _engine.Start();
            _eventLog.WriteEntry("MigrationService started");
        }

        protected override void OnStop()
        {
            _eventLog.WriteEntry("Try stopping service...");
            if (_engine != null)
            {
                var handle = _engine.Shutdown();
                _eventLog.WriteEntry("Sent shutdown signal...");
                handle.WaitOne();
            }
            _eventLog.WriteEntry("MigrationService stopped.");
        }
    }
}
