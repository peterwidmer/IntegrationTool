using IntegrationTool.SDK;
using IntegrationTool.SDK.Helpers;
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
            ExcelConnectionObject excelConnection = (ExcelConnectionObject)connection.GetConnection();
            ExcelWorksheet worksheet = excelConnection.Worksheet;

            // Write columnheaders into the worksheet
            foreach(var column in dataObject.Metadata.Columns)
            {
                worksheet.SetValue(1, column.Value.ColumnIndex + 1, column.Value.ColumnName);
            }

            // Write data into the worksheet
            for (int row = 0; row < dataObject.Count; row++)
            {
                object[] data = dataObject[row];
                for(int column =0; column < data.Length; column++)
                {
                    worksheet.SetValue(row + 2, column + 1, data[column]);
                }
            }

            excelConnection.Package.Save();

            
        }
    }
}
