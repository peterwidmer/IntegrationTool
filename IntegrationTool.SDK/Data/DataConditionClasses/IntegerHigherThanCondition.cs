using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{

    [DataConditionAttribute(ConditionType = TreatAsType.Int32, ConditionName = "Is higher than")]
    public class IntegerHigherThanCondition : IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            try
            {
                int dataConditionValue = Convert.ToInt32(dataCondition.Value);
                int objectValue = Convert.ToInt32(value);
                return objectValue > dataConditionValue;
            }
            catch
            {
                return false;
            }
        }
    }
}
