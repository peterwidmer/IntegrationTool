using IntegrationTool.Module.DeleteInDynamicsCrm.Logging.RecordLogList;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
                    "[EntityIds] NVARCHAR(MAX), " +
                    "[DeletionFault] NVARCHAR(MAX) " +
                    ");");
        }

        public void AddRecord(int record, string combinedBusinessKey, string entityIds, string deletionFault)
        {
            deletionFault = deletionFault.Length <= 2000 ? deletionFault : deletionFault.Substring(0, 2000);

            this.databaseInterface.ExecuteNonQuery(
                "INSERT INTO tblRecordLog (" +
                                "pkRecordLogId," +
                                "CombinedBusinessKey," +
                                "EntityIds," +
                                "DeletionFault," +
                                ") " +
                "values (" + record.ToString() + "," +
                       "'" + combinedBusinessKey + "'," +
                       "'" + entityIds + "'," +
                            "'" + deletionFault + "'," +
                             ")");
        }

        public ObservableCollection<RecordLog> ReadPagedRecords(int pageNumber)
        {
            ObservableCollection<RecordLog> recordLogs = new ObservableCollection<RecordLog>();

            string pagingSqlCommand =
                "SELECT " +
                "pkRecordLogId, " +
                "CombinedBusinessKey, " +
                "EntityIds, " +                
                "DeletionFault " +
                "FROM tblRecordLog " +
                "WHERE pkRecordLogId > " + pagingLastRecordLogId.ToString() + " " +
                "ORDER BY pkRecordLogId " +
                "LIMIT " + PAGINGSIZE;

            DbDataReader dataReader = this.databaseInterface.ExecuteQuery(pagingSqlCommand);
            while (dataReader.Read())
            {
                RecordLog recordLog = new RecordLog();
                recordLog.RecordLogId = dataReader.GetInt32(0);
                recordLog.CombinedBusinessKey = dataReader.GetString(1).Replace("##", " ");
                recordLog.EntityIds = dataReader.GetString(2).Replace("##", " ");
                recordLog.DeletionFault = dataReader.IsDBNull(3) ? "" : dataReader.GetValue(3).ToString();

                recordLogs.Add(recordLog);
            }

            return recordLogs;
        }

        public static LogSummary LoadLogSummary(IDatabaseInterface databaseInterface)
        {
            LogSummary logSummary = new LogSummary();

            object result = null;

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog");
            logSummary.NumberOfRecordsLoaded = result == null ? -1 : Convert.ToInt32(result);

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog where DeletionFault IS NULL");
            logSummary.NumberOfSuccessfulRecords = result == null ? -1 : Convert.ToInt32(result);

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog where DeletionFault IS NOT NULL");
            logSummary.NumberOfFailedRecords = result == null ? -1 : Convert.ToInt32(result);

            return logSummary;
        }
    }
}
