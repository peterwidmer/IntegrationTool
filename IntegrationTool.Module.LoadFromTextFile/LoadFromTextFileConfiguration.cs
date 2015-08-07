using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromTextFile
{
    public enum TextFileLoadType
    {
        OneRowPerLine,
        AllInOneRow
    }

    public class LoadFromTextFileConfiguration : SourceConfiguration
    {
        public string ColumnName { get; set; }
        public TextFileLoadType LoadType { get; set; }

        public LoadFromTextFileConfiguration()
        {
            if (String.IsNullOrEmpty(ColumnName)) { ColumnName = "TextFile"; }
        }
    }
}
