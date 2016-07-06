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
        TransformationType = StringTransformationType.SetValue,
        Param1Label = "Value",
        Param1Visibility = Visibility.Visible,
        Param2Label = "",
        Param2Visibility = Visibility.Hidden)]
    public class SetValue : ITransformationExecutor
    {
        public string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation)
        {
            return stringTransformation.Parameter1;
        }
    }
}
