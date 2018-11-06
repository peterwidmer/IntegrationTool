using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Diagram
{
    public class DesignerItemBase
    {
        private string _itemLabel;
        public Guid ID { get; set; }

        public string ItemLabel
        {
            get => String.IsNullOrEmpty(_itemLabel.Trim()) ? "-- No Label --" : _itemLabel.Trim();
            set => _itemLabel = value;
        }

        public ModuleDescription ModuleDescription { get; set; }
        public ItemState State { get; set; }
    }
}
