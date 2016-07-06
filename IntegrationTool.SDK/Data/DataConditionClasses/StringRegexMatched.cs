using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    [DataConditionAttribute(ConditionType = TreatAsType.String, ConditionName = "Regex Matches")]
    public class StringRegexMatched : IDataConditionCheck
    {
        public bool ConditionIsFullfilled(DataCondition dataCondition, object value)
        {
            string dataConditionValue = dataCondition.Value.ToString();
            string objectValue = value.ToString();

            return Regex.Match(objectValue, dataConditionValue).Success;
        }
    }
}
