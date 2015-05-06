using IntegrationTool.SDK.Data.DataConditionClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class AttributeImplementation
    {
        public Attribute Attribute { get; set; }
        public Type ImplementationType { get; set; }
        public IDataConditionCheck DataConditionCheck { get; set; }

        public AttributeImplementation(Attribute attribute, Type implementationType)
        {
            this.Attribute = attribute;
            this.ImplementationType = implementationType;
        }

    }
    public class AssemblyHelper
    {
        public static List<AttributeImplementation> LoadAllClassesImplementingSpecificAttribute<T>(Assembly assembly)
        {
            IEnumerable<Type> typesImplementingAttribute = GetTypesWithSpecificAttribute<T>(assembly);

            List<AttributeImplementation> attributeList = new List<AttributeImplementation>();
            foreach (Type type in typesImplementingAttribute)
            {
                var attribute = type.GetCustomAttribute(typeof(T));
                attributeList.Add(new AttributeImplementation(attribute, type));
            }

            return attributeList;
        }

        private static IEnumerable<Type> GetTypesWithSpecificAttribute<T>(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
