using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.SDK
{
    public class ModuleDescription
    {
        public Type ModuleType;
        public ModuleAttributeBase Attributes { get; set; }

        public ModuleDescription()
        {
            Attributes = new ModuleAttributeBase();
        }
    }
}
