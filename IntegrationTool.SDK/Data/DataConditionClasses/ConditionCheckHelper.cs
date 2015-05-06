using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    public class ConditionCheckHelper
    {
        public static bool ? AreObjectsNullEqual(object dataConditionValue, object value)
        {
            if (value == null && dataConditionValue == null)
            {
                return true;
            }

            if (dataConditionValue != null && value == null)
            {
                return false;
            }

            if (dataConditionValue == null && value != null)
            {
                return false;
            }

            return null;
        }
    }
}
