using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.DeleteInDynamicsCrm
{
    public enum DeleteInCrmMultipleFoundMode
    {
        DeleteAll,
        DeleteNone
    }

    public class DeleteInDynamicsCrmConfiguration : TargetConfiguration
    {
        public string EntityName { get; set; }
        public List<DataMapping> DeleteMapping { get; set; }
        public DeleteInCrmMultipleFoundMode MultipleFoundMode { get; set; }

        public DeleteInDynamicsCrmConfiguration()
        {
            this.DeleteMapping = new List<DataMapping>();
        }
    }
}
