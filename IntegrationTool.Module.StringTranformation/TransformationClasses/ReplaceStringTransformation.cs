using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.Module.StringTranformation.SDK.Enums;
using System;
using System.Windows;

namespace IntegrationTool.Module.StringTranformation.TransformationClasses
{
    [StringTransformationAttribute(
        TransformationType=StringTransformationType.Replace,
        Param1Label="Replace",
        Param1Visibility = Visibility.Visible,
        Param2Label="With",
        Param2Visibility= Visibility.Visible)]
    public class ReplaceStringTransformation : ITransformationExecutor
    {
        public string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation)
        {
            return inputString.Replace(stringTransformation.Parameter1, stringTransformation.Parameter2);
        }
    }
}
