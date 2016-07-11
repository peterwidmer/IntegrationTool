using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.Module.StringTranformation.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IntegrationTool.Module.StringTranformation.TransformationClasses
{
    [StringTransformationAttribute(
        TransformationType = StringTransformationType.Regex,
        Param1Label = "Pattern",
        Param1Visibility = Visibility.Visible,
        Param2Label = "Replacement",
        Param2Visibility = Visibility.Visible)]
    public class RegexReplace : ITransformationExecutor
    {
        public string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation)
        {
            Regex regex = new Regex(stringTransformation.Parameter1);
            return regex.Replace(inputString, stringTransformation.Parameter2);
        }
    }
}
