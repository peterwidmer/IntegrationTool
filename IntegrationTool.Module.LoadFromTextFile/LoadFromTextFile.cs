using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromTextFile
{
    [SourceModuleAttribute(Name = "LoadFromTextFile",
                           DisplayName = "Textfile",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(StringReader),
                           ConfigurationType = typeof(LoadFromTextFileConfiguration))]
    public class LoadFromTextFile : IModule, IDataSource
    {
        public LoadFromTextFileConfiguration Configuration { get; set; }
        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromTextFileConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromTextFileConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress, bool mappingPreview)
        {
            datastore.AddColumn(new ColumnMetadata("TextFile"));

            using (StringReader stringReader = connection.GetConnection() as StringReader)
            {
                switch (this.Configuration.LoadType)
                {
                    case TextFileLoadType.AllInOneRow:
                        datastore.AddData(new object[] { stringReader.ReadToEnd() });
                        break;

                    case TextFileLoadType.OneRowPerLine:
                        string line = null;
                        while (true)
                        {
                            line = stringReader.ReadLine();
                            if (line == null) { break; }

                            datastore.AddData(new object[] { line });
                        }
                        break;
                }
            }
            
        }
    }
}
