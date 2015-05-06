using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.StringTranformation
{
    public class StringTransformationConfiguration : TransformationConfiguration
    {
        public ObservableCollection<StringTranformation.SDK.StringTransformationParameter> Transformations { get; set; }
   
        public StringTransformationConfiguration()
        {
            Transformations = new ObservableCollection<StringTranformation.SDK.StringTransformationParameter>();
        }
    }
}
