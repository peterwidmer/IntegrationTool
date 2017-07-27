using IntegrationTool.Module.WriteToDynamicsCrm.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteToDynamicsCrm.SDK
{
    public static class Constants
    {
        public static List<ImportMode> CreateModes = new List<ImportMode>() { ImportMode.Create, ImportMode.All, ImportMode.AllChangedValuesOnly };
        public static List<ImportMode> UpdateModes = new List<ImportMode>() { ImportMode.Update, ImportMode.UpdateChangedValuesOnly, ImportMode.All, ImportMode.AllChangedValuesOnly };
    }
}
