using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.Execution.Models
{
    public struct ResolvedEntity
    {
        public Entity Value;

        public ResolvedEntity(Entity entity)
        {
            Value = entity;
        }
    }
}
