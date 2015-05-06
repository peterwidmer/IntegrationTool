using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data.DataConditionClasses
{
    public interface IDataConditionCheck
    {
        bool ConditionIsFullfilled(DataCondition dataCondition, object value);
    }
}
