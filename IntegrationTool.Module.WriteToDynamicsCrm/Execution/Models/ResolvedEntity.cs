using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution.Models
{
    public struct ResolvedEntity
    {
        public Guid EntityId;
        public string SerializedEntity;

        public ResolvedEntity(Guid entityId, string serializedEntity)
        {
            EntityId = entityId;
            SerializedEntity = serializedEntity;
        }
    }
}
