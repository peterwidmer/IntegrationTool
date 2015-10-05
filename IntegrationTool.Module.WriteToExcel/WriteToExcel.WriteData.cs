using IntegrationTool.SDK;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToExcel
{
    public partial class WriteToExcel
    {
        public void WriteData(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, SDK.Database.IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            ExcelWorksheet worksheet = (ExcelWorksheet)connection.GetConnection();

            foreach(var column in dataObject.Metadata.Columns)
            {
                worksheet.SetValue(1, column.Value.ColumnIndex + 1, column.Value.ColumnName);
            }

            for (int row = 0; row < dataObject.Count; row++)
            {
                object[] data = dataObject[row];
                for(int column =0; column < data.Length; column++)
                {
                    worksheet.SetValue(row + 2, column + 1, data[column]);
                }
            }

            
        }
    }
}
