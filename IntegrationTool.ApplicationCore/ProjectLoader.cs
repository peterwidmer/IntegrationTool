using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ProjectLoader
    {
        public static Project LoadFromPath(string projectPath, ApplicationInitializer appInitializer)
        {
            ModuleLoader moduleLoader = appInitializer.ModuleLoader;

            Type[] extraTypes = moduleLoader.GetModuleTypeList();

            Project project = Project.LoadFromFile(projectPath, extraTypes);
            project.Initialize(moduleLoader.Modules);

            return project;
        }
    }
}
