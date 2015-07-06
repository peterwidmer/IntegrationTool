using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.DeleteInDynamicsCrm.Logging
{
    public class Logger
    {
        private int pagingLastRecordLogId;
        private const int PAGINGSIZE = 100000000;

        private IDatabaseInterface databaseInterface;

        public Logger(IDatabaseInterface databaseInterface)
        {
            this.databaseInterface = databaseInterface;

            pagingLastRecordLogId = -1;
        }

        public void InitializeDatabase()
        {
            this.databaseInterface.ExecuteNonQuery(
                "CREATE TABLE [main].[tblRecordLog]( " +
                    "[pkRecordLogId] INTEGER PRIMARY KEY NOT NULL UNIQUE, " +
                    "[CombinedBusinessKey] NVARCHAR(256), " +
                    "[EntityId] NVARCHAR(36), " +
                    "[SuccessfulDeleted] BIT, " +
                    "[DeletionFault] NVARCHAR(512) " +
                    ");");
        }

        public void AddRecord(int record, string combinedBusinessKey, Guid entityId, bool successfulDeleted, string deletionFault)
        {
            this.databaseInterface.ExecuteNonQuery(
                "INSERT INTO tblRecordLog (" +
                                "pkRecordLogId," +
                                "CombinedBusinessKey," +
                                "SuccessfulDeleted," +
                                "DeletionFault," +
                                ") " +
                "values (" + record.ToString() + "," +
                       "'" + combinedBusinessKey + "'," +
                       "'" + entityId.ToString().Replace("{","").Replace("}","") + "'," +
                            (successfulDeleted ? "1" : "0") + "," +
                            "'" + deletionFault + "'," +
                             ")");
        }
    }
}
