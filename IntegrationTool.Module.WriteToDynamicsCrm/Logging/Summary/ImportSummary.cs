using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Logging.Models
{
    public class ImportSummary
    {
        public int NumberOfRecordsLoaded { get; set; }
        public int NumberOfSuccessfulRecords { get; set; }
        public int NumberOfFailedRecords { get; set; }

        public static ImportSummary Load(IDatabaseInterface databaseInterface)
        {
            ImportSummary importSummary = new ImportSummary();

            object result = null;

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog");
            importSummary.NumberOfRecordsLoaded = result == null ? -1 : Convert.ToInt32(result);

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog where WriteFault IS NULL");
            importSummary.NumberOfSuccessfulRecords = result == null ? -1 : Convert.ToInt32(result);

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog where WriteFault IS NOT NULL");
            importSummary.NumberOfFailedRecords = result == null ? -1 : Convert.ToInt32(result);

            return importSummary;
        }
    }
}
