using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class ItemWorker
    {
        public DesignerItemBase DesignerItem { get; set; }
        public StepConfigurationBase Configuration { get; set; }        
        public System.ComponentModel.BackgroundWorker BackgroundWorker { get; set; }

        public ItemWorker(DesignerItemBase designerItem, StepConfigurationBase configuration)
        {
            this.DesignerItem = designerItem;
            this.Configuration = configuration;
        }
    }
}
