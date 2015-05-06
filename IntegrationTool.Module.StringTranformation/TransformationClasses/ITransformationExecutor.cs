using IntegrationTool.Module.StringTranformation.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.StringTranformation.TransformationClasses
{
    public interface ITransformationExecutor
    {
        string ExecuteTransformation(string inputString, StringTransformationParameter stringTransformation);
    }
}
