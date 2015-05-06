using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK.GenericClasses
{
    public class NameDisplayName
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public NameDisplayName()
        {
        }

        public NameDisplayName(string name, string displayName)
        {
            this.Name = name;
            this.DisplayName = displayName;
        }
    }
}
