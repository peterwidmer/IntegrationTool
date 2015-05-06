using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public delegate void ReportProgressMethod(SimpleProgressReport progress);

    public class SimpleProgressReport
    {
        public string Message { get; set; }

        public SimpleProgressReport(string message)
        {
            this.Message = message;
        }
    }

    public class ProgressReport
    {
        public DesignerItemBase DesignerItem { get; set; }
        public ItemEvent State { get; set; }
        public string Message { get; set; }
        public object CustomObject { get; set; }
    }
}
