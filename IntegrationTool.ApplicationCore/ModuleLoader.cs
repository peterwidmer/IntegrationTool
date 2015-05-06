using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ModuleLoader
    {
        public List<ModuleDescription> Modules { get; set; }

        public ModuleLoader()
        {
            Modules = new List<ModuleDescription>();
        }

        public void LoadModules(string pathToLoadModuleFrom)
        {
            if(Directory.Exists(pathToLoadModuleFrom) == false)
            {
                return;
            }

            string [] filesInModulePath = Directory.GetFiles(pathToLoadModuleFrom);
            foreach(string file in filesInModulePath.Where(t=> t.EndsWith(".dll") && t.ToLower().Contains("microsoft") == false))
            {
                Assembly assembly = Assembly.LoadFrom(new FileInfo(file).FullName);
                List<Type> modulesInAssembly = GetTypesByInterface<IModule>(assembly);
                foreach(Type type in modulesInAssembly)
                {
                    ModuleDescription moduleDescription = new ModuleDescription();
                    moduleDescription.Attributes = (ModuleAttributeBase)type.GetCustomAttribute(typeof(ModuleAttributeBase));
                    moduleDescription.ModuleType = type;

                    this.Modules.Add(moduleDescription);
                }
            }
        }

        private static List<Type> GetTypesByInterface<T>(Assembly assembly)
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be an interface");

            return assembly.GetTypes()
                .Where(x => x.GetInterface(typeof(T).Name) != null)
                .Select(x => x).ToList<Type>();
        }

        public Type[] GetModuleTypeList()
        {
            List<Type> extraTypes = new List<Type>();
            foreach (ModuleDescription moduleDescription in this.Modules)
            {
                extraTypes.Add(moduleDescription.ModuleType);
                if (moduleDescription.Attributes.ConfigurationType != null)
                {
                    extraTypes.Add(moduleDescription.Attributes.ConfigurationType);
                }
            }

            return extraTypes.ToArray();
        }

    }
}
