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
        public ItemState State { get; set; }

        public ItemWorker(DesignerItemBase designerItem, ItemState state, StepConfigurationBase configuration)
        {
            this.DesignerItem = designerItem;
            this.State = state;
            this.Configuration = configuration;
        }
    }
}
