using IntegrationTool.Module.StringTranformation.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.StringTranformation.SDK
{
    public class StringTransformationParameter
    {
        public StringTransformationType TransformationType { get; set; }

        public string ColumnName { get; set; }

        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }
    }
}
