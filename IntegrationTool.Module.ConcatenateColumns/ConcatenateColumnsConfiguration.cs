using IntegrationTool.Module.ConcatenateColumns.SDK;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConcatenateColumns
{
    public class ConcatenateColumnsConfiguration : TransformationConfiguration
    {
        public ObservableCollection<ColumnConcatenation> ColumnConcatenations { get; set; }

        public ConcatenateColumnsConfiguration()
        {
            ColumnConcatenations = new ObservableCollection<ColumnConcatenation>();
        }
    }
}
