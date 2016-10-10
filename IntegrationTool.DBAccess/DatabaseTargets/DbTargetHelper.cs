using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.DBAccess
{
    public class DbTargetHelper
    {
        public static List<KeyValuePair<string, object>> BuildRecordIdentifiers(DbTargetCommonConfiguration configuration, object[] rowData, DataMetadata dataMetadata)
        {
            List<KeyValuePair<string, object>> recordIdentifiers = new List<KeyValuePair<string, object>>();

            foreach (var primaryKeyField in configuration.PrimaryKeyFields)
            {
                var primaryKeyMapping = configuration.Mapping.FirstOrDefault(t => t.Target == primaryKeyField);
                if (primaryKeyMapping == null)
                {
                    throw new Exception("The primarykey " + primaryKeyField + " is not mapped!");
                }

                var recordIdentifier = new KeyValuePair<string, object>(primaryKeyField, rowData[dataMetadata[primaryKeyMapping.Source].ColumnIndex]);
                recordIdentifiers.Add(recordIdentifier);
            }

            return recordIdentifiers;
        }
    }
}
