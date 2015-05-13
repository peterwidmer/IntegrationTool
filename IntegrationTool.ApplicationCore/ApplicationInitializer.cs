using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ApplicationInitializer
    {
        public ModuleLoader ModuleLoader { get; set; }

        public ApplicationInitializer()
        {
            string basePath = Assembly.GetExecutingAssembly().Location.Replace("IntegrationTool.ApplicationCore.dll", "");
            ModuleLoader = new ModuleLoader();
            ModuleLoader.LoadModules(basePath + @"Modules\Steps");
            ModuleLoader.LoadModules(basePath + @"Modules\Connections");
            ModuleLoader.LoadModules(basePath + @"Modules\Sources");
            ModuleLoader.LoadModules(basePath + @"Modules\Transformers");
            ModuleLoader.LoadModules(basePath + @"Modules\Targets");
        }
    }
}
