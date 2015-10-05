using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToExcel
{
    public enum WriteToExcelType
    {
        Simple
    }

    public class WriteToExcelConfiguration : TargetConfiguration
    {
        public WriteToExcelType WriteType { get; set; }
    }
}
