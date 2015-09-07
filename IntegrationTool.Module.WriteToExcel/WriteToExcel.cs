using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToExcel
{
    [TargetModuleAttribute(Name = "WriteToExcel",
                           DisplayName = "Excel",
                           ModuleType = ModuleType.Target,
                           GroupName = ModuleGroup.Target,
                           ConnectionType = typeof(ExcelWorkbook),
                           ConfigurationType = typeof(WriteToExcelConfiguration))]
    public partial class WriteToExcel : IModule, IDataTarget
    {
        public WriteToExcelConfiguration Configuration { get; set; }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configuration, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((WriteToExcelConfiguration)configuration, dataObject);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as WriteToExcelConfiguration;
        }        
    }
}
