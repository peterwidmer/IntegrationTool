using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    [DataConditionAttribute(ConditionType = TreatAsType.Int32, ConditionName = "Is not null", ValueVisibility = System.Windows.Visibility.Hidden)]
    public class IntegerNotNullCondition : IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            if (value != null && String.IsNullOrEmpty(value.ToString()) == false && value != DBNull.Value)
            {
                return true;
            }

            return false;
        }
    }
}
