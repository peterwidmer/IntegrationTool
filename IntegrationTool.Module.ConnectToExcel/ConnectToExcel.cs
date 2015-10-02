using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Module.ModuleAttributes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToExcel
{
    [ConnectionModuleAttribute(
        DisplayName = "Excel",
        Name = "ConnectToExcel",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(ExcelWorkbook),
        ConfigurationType=typeof(ConnectToExcelConfiguration))]
    public class ConnectToExcel : IModule, IConnection
    {
        public ConnectToExcelConfiguration Configuration { get; set; }

        public object GetConnection()
        {
            switch (this.Configuration.ConnectionType)
            {
                case ExcelConnectionType.ExistingFileAndSheet:
                    FileInfo file = new FileInfo(this.Configuration.FilePath);
                    if (file.Exists == false)
                        throw new FileNotFoundException("Error in LoadData within Data-Source Module \"Excel\": File " + file.FullName + " could not be found.");

                    ExcelPackage pck = new ExcelPackage(file);
                    var worksheet = pck.Workbook.Worksheets.Where(t => t.Name == this.Configuration.SheetName).FirstOrDefault();
                    if (worksheet == null)
                        throw new Exception("Sheet with name " + this.Configuration.SheetName + " could not be found");

                    return worksheet;

                case ExcelConnectionType.NewFileAndSheet:
                    string fullFilePath = this.Configuration.FilePath + this.Configuration.FileName;
                    if (File.Exists(fullFilePath))
                    {
                        File.Delete(fullFilePath);
                    }

                    ExcelPackage newPackage = new ExcelPackage(new FileInfo(fullFilePath));
                    var newWorksheet = newPackage.Workbook.Worksheets.Add(this.Configuration.SheetName);
                    newPackage.Save();

                    return newWorksheet;
            }

            throw new NotImplementedException();
            
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToExcelConfiguration)configurationBase);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToExcelConfiguration;
        }
    }
}
