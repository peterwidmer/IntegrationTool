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
        TransformationType = StringTransformationType.PadLeft,
        Param1Label = "Total width",
        Param1Visibility = Visibility.Visible,
        Param2Label = "Padding character",
        Param2Visibility = Visibility.Visible)]
    public class PadLeftTransformation : ITransformationExecutor
    {
        public string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation)
        {
            return inputString.PadLeft(Convert.ToInt32(stringTransformation.Parameter1), stringTransformation.Parameter2[0]);
        }
    }
}
