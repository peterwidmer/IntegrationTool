using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data
{
    public class DatastoreHelper
    {
        public static DataTable ConvertDatastoreToTable(IDatastore datastore, int numberOfRecordsToLoad, string [] columnsToRemove = null, int [] rowsNumbersToRemove = null) 
        {
            DataTable dt = new DataTable();
            foreach (var column in datastore.Metadata.Columns.OrderBy(t=>t.Value.ColumnIndex))
            {
                dt.Columns.Add(column.Value.ColumnName);
            }

            for (int i = 0; i < datastore.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int iCol = 0; iCol < dt.Columns.Count; iCol++)
                {
                    if (datastore[i][iCol] == null || datastore[i][iCol] == DBNull.Value)
                    {
                        dr[iCol] = "";
                    }
                    else
                    {
                        dr[iCol] = datastore[i][iCol].ToString();
                    }
                }
                dt.Rows.Add(dr);

                if (dt.Rows.Count == numberOfRecordsToLoad) { break; }
            }

            if (columnsToRemove != null)
            {
                foreach (string columnToRemove in columnsToRemove)
                {
                    if (dt.Columns.Contains(columnToRemove))
                    {
                        dt.Columns.Remove(columnToRemove);
                    }
                }
            }

            if(rowsNumbersToRemove != null)
            {
                foreach(int rowIndex in rowsNumbersToRemove.OrderByDescending( t=> t))
                {
                    dt.Rows.RemoveAt(rowIndex);
                }
            }
            return dt;
        }
    }
}
