using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Data.DataConditionClasses;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class DataObject : IDatastore
    {
        public Dictionary<string, AttributeImplementation> DataConditionAttributes;
        public DataMetadata Metadata { get; set; }
        private List<object[]> data;
        private DataFilter activeDataFilter;

        public DataObject()
        {
            Metadata = new DataMetadata();
            data = new List<object[]>();            
        }

        public void InitializeDatastore(List<AttributeImplementation> dataConditionAttributes)
        {
            DataConditionAttributes = new Dictionary<string, AttributeImplementation>();
            foreach (var attributeImplementation in dataConditionAttributes)
            {
                attributeImplementation.DataConditionCheck = Activator.CreateInstance(attributeImplementation.ImplementationType) as IDataConditionCheck;
                
                DataConditionAttribute attribute = attributeImplementation.Attribute as DataConditionAttribute;
                this.DataConditionAttributes.Add(attribute.ConditionName + attribute.ConditionType.ToString(), attributeImplementation);
            }
        }

        public List<NameDisplayName> GetDistinctValuesOfColumn(string columnName)
        {
            List<string> distinctValues = new List<string>();
            var column = this.Metadata.Columns.Values.Where(t => t.ColumnName == columnName).FirstOrDefault();
            if(column == null)
            {
                throw new Exception("Column " + columnName + " does not exist in the DataObject");
            }

            foreach(var objArray in this.data)
            {
                string value = objArray[column.ColumnIndex]?.ToString();
                if(distinctValues.Contains(value) == false)
                {
                    distinctValues.Add(value);
                }
            }

            List<NameDisplayName> returnList = new List<NameDisplayName>();
            foreach(string value in distinctValues.OrderBy(t=> t))
            {
                returnList.Add(new NameDisplayName(value, value));
            }

            return returnList;
        }

        public void AddColumn(ColumnMetadata columnMetadata)
        {
            if(this.Metadata.Columns.Count == 0)
            {
                columnMetadata.ColumnIndex = 0;
            }
            else
            {
                columnMetadata.ColumnIndex = this.Metadata.Columns.Max(t => t.Value.ColumnIndex) + 1;
            }

            this.Metadata.Columns.Add(columnMetadata.ColumnName, columnMetadata);

            for(int i=0; i < data.Count; i++)
            {
                if(data[i].Length < this.Metadata.Columns.Count)
                {
                    object[] newRecord = new object[this.Metadata.Columns.Count];
                    data[i].CopyTo(newRecord, 0);
                    data[i] = newRecord;
                }
            }
        }

        public void RemoveColumn(string columnName)
        {
            if (this.Metadata.Columns.Count == 0) { throw new ArgumentException("Datastore does not contain any columns to remove"); }
            if (this.Metadata.Columns.ContainsKey(columnName) == false) { throw new ArgumentOutOfRangeException("Column " + columnName + " does not exist in the datastore!"); }
            
            ColumnMetadata removedColumnMetadata = this.Metadata.Columns[columnName];
            this.Metadata.Columns.Remove(columnName);

            // Remove column from each row
            for(int rowIndex =0; rowIndex < this.data.Count; rowIndex++)
            {
                object [] objectArray = this.data[rowIndex];
                this.data[rowIndex] = new object[objectArray.Length - 1];
                int indexNewObject = 0;
                for(int index=0; index < objectArray.Length; index++)
                {
                    if (index == removedColumnMetadata.ColumnIndex) { continue; }
                    this.data[rowIndex][indexNewObject] = objectArray[index];
                    indexNewObject++;
                }
            }

            // Re-Index remaining columns
            foreach(KeyValuePair<string, ColumnMetadata> columnMetadata in this.Metadata.Columns.Where(t=> t.Value.ColumnIndex > removedColumnMetadata.ColumnIndex))
            {
                columnMetadata.Value.ColumnIndex--;
            }

            
        }

        public void AddData(object[] data)
        {
            this.data.Add(data);
        }

        public void RemoveDataAt(int index)
        {
            this.data.RemoveAt(index);
        }

        public void SetValue(int index, int column, object value)
        {
            if (this.activeDataFilter == null || this.activeDataFilter.FilterType == null)
            {
                this.data[index][column] = value;
            }
            else
            {
                bool isRecordInFilter = this.activeDataFilter.IsRecordInFilter(this.data[index], this.Metadata, this.DataConditionAttributes);
                if (isRecordInFilter)
                {
                    this.data[index][column] = value;
                }
            }
        }

        public void ApplyFilter(DataFilter dataFilter)
        {
            this.activeDataFilter = dataFilter;
        }

        public void ClearFilter()
        {
            this.activeDataFilter = null;
        }

        public object[] this[int index]
        {
            get { return this.data[index]; }
        }

        public int Count
        {
            get
            {
                return this.data.Count;
            }
        }
    }
}
