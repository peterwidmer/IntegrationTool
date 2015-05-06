using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromExcel
{
    [SourceModuleAttribute(Name="LoadFromExcel", 
                           DisplayName="Excel",
                           ModuleType=ModuleType.Source,
                           ConnectionType = typeof(ExcelWorkbook),
                           ConfigurationType= typeof(LoadFromExcelConfiguration))]
    public class LoadFromExcel : IModule, IDataSource
    {
        public LoadFromExcelConfiguration Configuration { get; set; }

        public LoadFromExcel()
        {
            Configuration = new LoadFromExcelConfiguration();
        }

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress)
        {
            ExcelWorksheet worksheet = (ExcelWorksheet)connection.GetConnection();

            for(int rowNumber=1; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
            {
                // If first row, the build MetaData
                if(rowNumber == 1)
                {
                    for (int columnNumber = 1; columnNumber <= worksheet.Dimension.End.Column; columnNumber++)
                    {
                        string columnName = worksheet.GetValue(rowNumber, columnNumber).ToString();
                        datastore.AddColumnMetadata(new ColumnMetadata(columnNumber - 1, columnName));
                    }
                    continue;
                }

                // All other rows go to the data
                object [] rowData = new object [worksheet.Dimension.End.Column];           
                for(int columnNumber=1; columnNumber <= worksheet.Dimension.End.Column; columnNumber++)
                {
                    rowData[columnNumber-1] = worksheet.GetValue(rowNumber, columnNumber);
                }
                datastore.AddData(rowData);

                if (StatusHelper.MustShowProgress(rowNumber - 1, worksheet.Dimension.End.Row) == true)
                {
                    reportProgress(new SimpleProgressReport("Loaded " + rowNumber + " of " + worksheet.Dimension.End.Row + " records"));
                }
            }
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromExcelConfiguration)configurationBase);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromExcelConfiguration;
        }
    }
}
