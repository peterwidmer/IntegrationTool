using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using IntegrationTool.SDK.Controls.Generic;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Logging
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
                    "[ImportMode] INT, " +
                    "[WriteFault] NVARCHAR(512) " +
                    ");");
        }

        public void AddRecord(int record)
        {
            this.databaseInterface.ExecuteNonQuery(
                "INSERT INTO tblRecordLog (pkRecordLogId) " +
                "values (" + record.ToString() + ")");
        }

        public void SetBusinessKeyAndImportTypeForRecord(int record, string businessKey, ImportMode importMode)
        {
            this.databaseInterface.ExecuteNonQuery(
                "Update tblRecordLog set CombinedBusinessKey = '" + SafeString(businessKey) + "', " +
                "ImportMode=" + (int)importMode + " " +
                "Where pkRecordLogId = " + record);
        }

        public void SetWriteFault(int record, string writeFault)
        {
            this.databaseInterface.ExecuteNonQuery(
                "Update tblRecordLog set WriteFault = '" + SafeString(writeFault) + "' " +
                "Where pkRecordLogId = " + record);
        }

        private string SafeString(string value)
        {
            value = value.Replace("'", "\"");

            return value;
        }

        public ObservableCollection<RecordLog> ReadPagedRecords(int pageNumber)
        {
            ObservableCollection<RecordLog> recordLogs = new ObservableCollection<RecordLog>();

            string pagingSqlCommand = 
                "SELECT " + 
                "pkRecordLogId, " +
                "CombinedBusinessKey, " +
                "ImportMode, " +
                "WriteFault " +
                "FROM tblRecordLog " +
                "WHERE pkRecordLogId > " + pagingLastRecordLogId.ToString() + " " +
                "ORDER BY pkRecordLogId " +
                "LIMIT " + PAGINGSIZE;

            DbDataReader dataReader = this.databaseInterface.ExecuteQuery(pagingSqlCommand);
            while(dataReader.Read())
            {
                RecordLog recordLog = new RecordLog();
                recordLog.RecordLogId = dataReader.GetInt32(0);
                recordLog.CombinedBusinessKey = dataReader.GetString(1).Replace("##", " ");
                recordLog.ImportMode = "";
                if(dataReader.IsDBNull(2) == false)
                {
                    int importMode = dataReader.GetInt32(2);
                    recordLog.ImportMode = ((ImportMode)importMode).ToString();
                }
                recordLog.WriteError = dataReader.IsDBNull(3) ? "" : dataReader.GetValue(3).ToString();

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

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog where WriteFault IS NULL");
            logSummary.NumberOfSuccessfulRecords = result == null ? -1 : Convert.ToInt32(result);

            result = databaseInterface.ExecuteScalar("Select count(*) from tblRecordLog where WriteFault IS NOT NULL");
            logSummary.NumberOfFailedRecords = result == null ? -1 : Convert.ToInt32(result);

            return logSummary;
        }

    }
}
