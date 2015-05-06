using IntegrationTool.Module.StringTranformation.SDK.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.StringTranformation.SDK
{
    public class Helpers
    {
        public static Dictionary<StringTransformationType, Type> LoadAllTransformationTypes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            IEnumerable<Type> typesImplementingAttribute = GetTypesWithStringTransformationAttribute(assembly);

            Dictionary<StringTransformationType, Type> stringTransformationMapping = new Dictionary<StringTransformationType, Type>();

            foreach (Type type in typesImplementingAttribute)
            {
                StringTransformationAttribute attribute = (StringTransformationAttribute)type.GetCustomAttribute(typeof(StringTransformationAttribute));
                stringTransformationMapping.Add(attribute.TransformationType, type);
            }

            return stringTransformationMapping;
        }

        public static List<StringTransformationAttribute> LoadAllTransformationClasses()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            IEnumerable<Type> typesImplementingAttribute = GetTypesWithStringTransformationAttribute(assembly);

            List<StringTransformationAttribute> attributeList = new List<StringTransformationAttribute>();
            foreach (Type type in typesImplementingAttribute)
            {
                StringTransformationAttribute attribute = (StringTransformationAttribute)type.GetCustomAttribute(typeof(StringTransformationAttribute));
                attributeList.Add(attribute);
            }

            return attributeList;
        }

        private static IEnumerable<Type> GetTypesWithStringTransformationAttribute(Assembly assembly)
        {
            foreach(Type type in assembly.GetTypes()) {
                if (type.GetCustomAttributes(typeof(StringTransformationAttribute), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
