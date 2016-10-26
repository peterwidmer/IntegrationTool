using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DatabaseHelper
    {
        public static DataTable ConvertDataReaderToDataTable(DbDataReader dataReader)
        {
            DataTable datatable = new DataTable();
            DataTable schemaTable = dataReader.GetSchemaTable();

            foreach (DataRow myRow in schemaTable.Rows)
            {
                DataColumn myDataColumn = new DataColumn();
                //myDataColumn.DataType = myRow.GetType();
                myDataColumn.ColumnName = myRow[0].ToString();
                datatable.Columns.Add(myDataColumn);
            }
            while (dataReader.Read())
            {
                DataRow myDataRow = datatable.NewRow();
                for (int i = 0; i < schemaTable.Rows.Count; i++)
                {
                    try
                    {
                        myDataRow[i] = dataReader[i].ToString();
                    }
                    catch {}
                }
                datatable.Rows.Add(myDataRow);
                myDataRow = null;
            }
            schemaTable = null;
            return datatable;
        }
    }
}
