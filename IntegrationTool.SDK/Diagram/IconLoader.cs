using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.SDK.Diagram
{
    public class IconLoader
    {
        public static Image GetFromAssembly(Assembly assembly, string iconName)
        {
            string[] sztr = assembly.GetManifestResourceNames();
            string resource = assembly.GetManifestResourceNames().Where(t=> t.ToLower().EndsWith("." + iconName.ToLower())).FirstOrDefault();

            if (String.IsNullOrEmpty(resource) == false)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resource))
                {
                    using (StreamReader streamReader = new StreamReader(stream))
                    {
                        StringReader stringReader = new StringReader(streamReader.ReadToEnd());
                        System.Windows.Controls.Image db = (System.Windows.Controls.Image)System.Windows.Markup.XamlReader.Load(System.Xml.XmlReader.Create(stringReader));
                        return db;
                    }
                }
            }

            throw new Exception("Could not find Icon.xml in Assembly " + assembly.FullName);
        }
    }
}
