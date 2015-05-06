using IntegrationTool.DiagramDesigner;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ProjectDesigner.Classes
{
    public class StepProperty
    {
        public DesignerItem DesignerItem { get; set; }
        public StepConfigurationBase Configuration {get; set; }
    }
}
