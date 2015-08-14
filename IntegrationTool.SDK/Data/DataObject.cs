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
                string value = objArray[column.ColumnIndex].ToString();
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
            
            ColumnMetadata columnMetadata = this.Metadata.Columns[columnName];
            this.Metadata.Columns.Remove(columnName);

            foreach(var objectArray in this.data)
            {
                object [] newObject = new object [objectArray.Length -1];
                int indexNewObject = 0;
                for(int index=0; index < objectArray.Length; index++)
                {
                    if (index == columnMetadata.ColumnIndex) { continue; }
                    newObject[indexNewObject] = objectArray[index];
                    indexNewObject++;
                }
            }
        }

        public void AddData(object[] data)
        {
            this.data.Add(data);
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
