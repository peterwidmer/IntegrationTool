using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IntegrationTool.Module.ConnectToTextFile
{
    [ConnectionModuleAttribute(
        DisplayName = "Textfile",
        Name = "ConnectToTextFile",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(StringReader),
        ConfigurationType = typeof(ConnectToTextFileConfiguration))]
    public class ConnectToTextFile : IModule, IConnection
    {
        public ConnectToTextFileConfiguration Configuration;

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToTextFileConfiguration;
        }

        public UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToTextFileConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            if(File.Exists(this.Configuration.FilePath) == false)
                throw new FileNotFoundException("Error in \"Connect To TextFile-Module\". File:\n" + this.Configuration.FilePath + "\ncould not be found.");

            using (StreamReader sr = new StreamReader(this.Configuration.FilePath))
            {
                return new StringReader(sr.ReadToEnd());
            }
        }
    }
}
