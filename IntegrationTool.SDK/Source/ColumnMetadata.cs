using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class ColumnMetadata
    {
        public ColumnMetadata()
        {

        }

        public ColumnMetadata(int columnIndex, string columnName)
        {
            this.ColumnIndex = columnIndex;
            this.ColumnName = columnName;
        }

        /// <summary>
        /// Name of the Column
        /// </summary>
        public string ColumnName { get; set; }
        
        /// <summary>
        /// Index of the column in the object-array
        /// </summary>
        public int ColumnIndex { get; set; }
    }
}
