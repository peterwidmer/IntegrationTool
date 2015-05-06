using IntegrationTool.Module.StringTranformation.SDK;
using IntegrationTool.Module.StringTranformation.SDK.Enums;
using IntegrationTool.Module.StringTranformation.TransformationClasses;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.StringTranformation
{
    public partial class StringTransformation
    {
        public void TransformData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            Dictionary<StringTransformationType, Type> stringTransformationMapping = Helpers.LoadAllTransformationTypes();

            foreach(var transformation in this.Configuration.Transformations)
            {
                reportProgress(new SimpleProgressReport("Start stringtranformation of type " + transformation.TransformationType + " on column " + transformation.ColumnName));
                
                if (dataObject.Metadata.Columns.Where(t => t.ColumnName == transformation.ColumnName).Count() == 0)
                {
                    throw new Exception("Column " + transformation.ColumnName + " was not found in the sourcedata");
                }

                int columnIndex = dataObject.Metadata.Columns.Where(t => t.ColumnName == transformation.ColumnName).First().ColumnIndex;
                ITransformationExecutor transformer = Activator.CreateInstance(stringTransformationMapping[transformation.TransformationType]) as ITransformationExecutor;

                for(int i=0; i < dataObject.Count; i++)
                {
                    string transformedValue = transformer.ExecuteTransformation(dataObject[i][columnIndex].ToString(), transformation);
                    dataObject.SetValue(i, columnIndex, transformedValue);
                }

                reportProgress(new SimpleProgressReport("Finished stringtranformation of type " + transformation.TransformationType + " on column " + transformation.ColumnName));
            }
        }
    }
}
