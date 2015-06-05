using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConcatenateColumns
{
    public partial class ConcatenateColumns
    {
        public void TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            foreach (var transformation in this.Configuration.ColumnConcatenations)
            {
                reportProgress(new SimpleProgressReport("Start column-cocatenation"));

                dataObject.AddColumnMetadata(new ColumnMetadata(dataObject.Metadata.Columns.Count, transformation.OutputColumn));
                
                int leftColumnIndex = dataObject.Metadata.Columns.Values.Where(t => t.ColumnName == transformation.LeftColumn).First().ColumnIndex;
                int rightColumnIndex = dataObject.Metadata.Columns.Values.Where(t => t.ColumnName == transformation.RightColumn).First().ColumnIndex;
                
                for (int i = 0; i < dataObject.Count; i++)
                {
                    string leftValue = (dataObject[i][leftColumnIndex] == null || dataObject[i][leftColumnIndex] == DBNull.Value) ? "" : dataObject[i][leftColumnIndex].ToString();
                    string rightValue = (dataObject[i][rightColumnIndex] == null || dataObject[i][rightColumnIndex] == DBNull.Value) ? "" : dataObject[i][rightColumnIndex].ToString();
                    string concatenatedValue = leftValue + transformation.ColumnSeparation + rightValue;

                    dataObject.SetValue(i, (int)(dataObject.Metadata.Columns.Count - 1), concatenatedValue);
                }

                reportProgress(new SimpleProgressReport("Finished column concatenation"));
            }
        }
    }
}
