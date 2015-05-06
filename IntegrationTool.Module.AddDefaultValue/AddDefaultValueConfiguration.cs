using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.AddDefaultValue
{
    public class AddDefaultValueConfiguration : TransformationConfiguration
    {
        public ObservableCollection<DefaultValue> DefaultValues { get; set; }

        public AddDefaultValueConfiguration()
        {
            DefaultValues = new ObservableCollection<DefaultValue>();
        }

    }
}
