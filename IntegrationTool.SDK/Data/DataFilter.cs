using IntegrationTool.SDK.Data.DataConditionClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.Data
{
    public enum DataFilterType
    {
        AND, OR
    }

    public partial class DataFilter
    {
        public DataFilterType ? FilterType { get; set; }

        public ObservableCollection<DataCondition> DataConditions { get; set; }

        public ObservableCollection<DataFilter> Filters { get; set; }

        public DataFilter()
        {
            DataConditions = new ObservableCollection<DataCondition>();
            Filters = new ObservableCollection<DataFilter>();
        }

        public bool IsRecordInFilter(object[] record, DataMetadata dataMetadata, Dictionary<string, AttributeImplementation> dataConditionAttributes)
        {
            bool conditionsValid = CheckDataConditions(record, dataMetadata, dataConditionAttributes);
            if(conditionsValid == false)
            {
                return false;
            }

            foreach(DataFilter subFilter in this.Filters)
            {
                bool isRecordInSubFilter = IsRecordInSubFilter(record, dataMetadata, subFilter, dataConditionAttributes);
                if(isRecordInSubFilter == false)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckDataConditions(object[] record, DataMetadata dataMetadata, Dictionary<string, AttributeImplementation> dataConditionAttributes)
        {
            bool conditionsResult = false;
            foreach (DataCondition dataCondition in this.DataConditions)
            {
                int columnIndex = dataMetadata[dataCondition.ColumnName].ColumnIndex;
                object objectValue = record[columnIndex];
                AttributeImplementation attributeImplentation = dataConditionAttributes[dataCondition.ConditionType + dataCondition.TreatAsType.ToString()];
                
                conditionsResult = attributeImplentation.DataConditionCheck.ConditionIsFullfilled(dataCondition, objectValue);
                
                switch (this.FilterType.Value)
                {
                    case DataFilterType.AND:                        
                        if (conditionsResult == false)
                        {
                            return false;
                        }
                        break;

                    case DataFilterType.OR:
                        if (conditionsResult == true)
                        {
                            return true;
                        }
                        break;

                    default:
                        throw new NotImplementedException("Filtertype " + this.FilterType.Value + " is not implemented!");
                }
            }

            return this.FilterType.Value == DataFilterType.OR ? false : true;
        }

        private bool IsRecordInSubFilter(object[] record, DataMetadata dataMetadata, DataFilter subFilter, Dictionary<string, AttributeImplementation> dataConditionAttributes)
        {
            return subFilter.IsRecordInFilter(record, dataMetadata, dataConditionAttributes);
        }
    }
}
