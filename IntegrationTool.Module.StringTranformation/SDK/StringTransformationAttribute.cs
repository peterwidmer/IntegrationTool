using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTool.Module.StringTranformation.SDK.Enums;

namespace IntegrationTool.Module.StringTranformation.SDK
{
    [AttributeUsage(AttributeTargets.All)]
    public class StringTransformationAttribute : Attribute
    {
        /// <summary>
        /// Replace, TrimStart, TrimEnd... etc
        /// </summary>
        public StringTransformationType TransformationType { get; set; }

        public string Param1Label { get; set; }
        public System.Windows.Visibility Param1Visibility { get; set; }
        
        public string Param2Label { get; set; }
        public System.Windows.Visibility Param2Visibility { get; set; }

    }
}
