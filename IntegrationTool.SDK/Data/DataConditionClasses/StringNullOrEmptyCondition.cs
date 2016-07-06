using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    [DataConditionAttribute(ConditionType = TreatAsType.String, ConditionName = "Is null or empty", ValueVisibility=System.Windows.Visibility.Hidden)]
    public class StringNullOrEmptyCondition : IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            return (value == null || String.IsNullOrEmpty(value.ToString()) || value == DBNull.Value);
        }
    }
}
