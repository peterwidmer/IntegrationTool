using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    [DataConditionAttribute(ConditionType = TreatAsType.Int32, ConditionName = "Equals")]
    public class IntegerEqualCondition: IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            bool ? nullEqualCheck = ConditionCheckHelper.AreObjectsNullEqual(dataCondition.Value, value);
            if(nullEqualCheck != null)
            {
                return (bool)nullEqualCheck;
            }

            try
            {
                int dataConditionValue = Convert.ToInt32(dataCondition.Value);
                int objectValue = Convert.ToInt32(value);
                return dataConditionValue == objectValue;
            }
            catch
            {
                return false;
            }
        }
    }
}
