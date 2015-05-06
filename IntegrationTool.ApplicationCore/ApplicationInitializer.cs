using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ApplicationInitializer
    {
        public ModuleLoader ModuleLoader { get; set; }

        public ApplicationInitializer()
        {
            ModuleLoader = new ModuleLoader();
            ModuleLoader.LoadModules(@"Modules\Steps");
            ModuleLoader.LoadModules(@"Modules\Connections");
            ModuleLoader.LoadModules(@"Modules\Sources");
            ModuleLoader.LoadModules(@"Modules\Transformers");
            ModuleLoader.LoadModules(@"Modules\Targets");
        }
    }
}
