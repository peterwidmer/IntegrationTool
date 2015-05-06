using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    public class DataConditionAttribute : Attribute
    {
        public TreatAsType ConditionType { get; set; }

        public string ConditionName { get; set; }

        public System.Windows.Visibility ValueVisibility { get; set; }

        public DataConditionAttribute()
        {
            ValueVisibility = System.Windows.Visibility.Visible;
        }
    }
}
