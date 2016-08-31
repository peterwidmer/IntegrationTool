using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Diagram
{
    public class DesignerItemBase
    {
        public Guid ID { get; set; }
        public string ItemLabel { get; set; }
        public ModuleDescription ModuleDescription { get; set; }
        public ItemState State { get; set; }
    }
}
