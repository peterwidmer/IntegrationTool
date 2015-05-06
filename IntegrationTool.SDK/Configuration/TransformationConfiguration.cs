using IntegrationTool.SDK.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class TransformationConfiguration : StepConfigurationBase
    {
        public DataFilter DataFilter { get; set; }

        public TransformationConfiguration()
        {
            DataFilter = new DataFilter();
        }
    }
}
