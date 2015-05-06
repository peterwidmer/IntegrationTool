using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.Module.StringTranformation.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrationTool.Module.StringTranformation.TransformationClasses
{
    [StringTransformationAttribute(
        TransformationType = StringTransformationType.PadRight,
        Param1Label = "Total width",
        Param1Visibility = Visibility.Visible,
        Param2Label = "Padding character",
        Param2Visibility = Visibility.Visible)]
    public class PadRightTransformation : ITransformationExecutor
    {
        public string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation)
        {
            return inputString.PadRight(Convert.ToInt32(stringTransformation.Parameter1), stringTransformation.Parameter2[0]);
        }
    }
}
