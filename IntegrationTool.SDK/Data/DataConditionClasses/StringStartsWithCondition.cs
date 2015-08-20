using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    [DataConditionAttribute(ConditionType = TreatAsType.String, ConditionName = "Starts With")]
    public class StringStartsWithCondition : IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            string dataConditionValue = dataCondition.Value.ToString();
            string objectValue = value.ToString();

            return objectValue.StartsWith(dataConditionValue);
        }
    }
}
