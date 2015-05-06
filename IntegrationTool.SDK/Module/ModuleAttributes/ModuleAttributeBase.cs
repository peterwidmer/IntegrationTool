using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    [AttributeUsage(AttributeTargets.All)]
    public class ModuleAttributeBase : Attribute
    {
        /// <summary>
        /// Unique name for the module
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name to show, when the module must be listed somewhere
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Is the module a Connection, Source, Transformer, Target
        /// </summary>
        public ModuleType ModuleType { get; set; }

        /// <summary>
        /// By default false, except for the main-flow module
        /// </summary>
        public bool ContainsSubConfiguration { get; set; }

        /// <summary>
        /// Every module must have a configuration object.
        /// </summary>
        public Type ConfigurationType { get; set; }

        /// <summary>
        /// Defines if a connection must be configured for the module
        /// </summary>
        public bool RequiresConnection { get; set; }

        /// <summary>
        /// The type of the required connection or 
        /// in case of the connection the connectiontype which is provided
        /// </summary>
        public Type ConnectionType { get; set; }

        /// <summary>
        /// In which group in the toolbox treeview should the module be displayed?
        /// </summary>
        public ModuleGroup GroupName { get; set; }

        public ModuleAttributeBase()
        {
            RequiresConnection = true;
        }
    }
}
