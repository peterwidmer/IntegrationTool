using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Helpers
{
    public class ExcelConnectionObject
    {
        public ExcelPackage Package { get; set; }
        public ExcelWorksheet Worksheet { get; set; }
    }
}
