using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTool.SDK
{
    public class ItemLog
    {
        public Guid DesignerItemId { get; set; }
        public string DesignerItemDisplayName { get; set; }
        public string ModuleDescriptionName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string DatabasePath { get; set; }
        public List<ItemLog> SubFlowLogs { get; set; }

        [XmlIgnore]
        public bool ExecutionSuccessful
        {
            get
            {
                return String.IsNullOrEmpty(this.ExecutionError);
            }
        }

        /// <summary>
        /// Contains the errors returned by an exception.
        /// </summary>
        public string ExecutionError { get; set; }

        public ItemLog()
        {
            SubFlowLogs = new List<ItemLog>();
        }

        public static ItemLog CreateNew(Guid designerItemId, string designerItemDisplayName, string moduleDescriptionName, DateTime startTime)
        {
            ItemLog itemLog = new ItemLog()
            {
                DesignerItemId = designerItemId,
                DesignerItemDisplayName = designerItemDisplayName,
                ModuleDescriptionName = moduleDescriptionName,
                StartTime = startTime
            };

            return itemLog;
        }
    }
}
