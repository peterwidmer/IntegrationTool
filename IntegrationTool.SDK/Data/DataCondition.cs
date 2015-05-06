using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data
{
    public enum TreatAsType
    {
        String, Int32
    }

    public class DataCondition
    {
        public string ColumnName { get; set; }
        public int ColumnIndex { get; set; }

        public TreatAsType TreatAsType { get; set; }
        public String ConditionType { get; set; }

        public object Value { get; set; }

        public DataCondition() { }

        public DataCondition(string columnName, TreatAsType treatAsType, string dataConditionType, object value)
        {
            this.ColumnName = columnName;
            this.TreatAsType = treatAsType;
            this.ConditionType = dataConditionType;
            this.Value = value;
        }
    }
}
