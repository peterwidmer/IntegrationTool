using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    [DataConditionAttribute(ConditionType=TreatAsType.String, ConditionName="Equals")]
    public class StringEqualCondition : IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            bool? nullEqualCheck = ConditionCheckHelper.AreObjectsNullEqual(dataCondition.Value, value);
            if (nullEqualCheck != null)
            {
                return (bool)nullEqualCheck;
            }

            string dataConditionValue = dataCondition.Value.ToString();
            string objectValue = value.ToString();

            return dataConditionValue.Equals(objectValue);
        }
    }
}
