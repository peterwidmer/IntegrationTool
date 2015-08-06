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
            Configuration = new LoadFromTextFileConfiguration();
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            throw new NotImplementedException();
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress)
        {
            StringReader stringReader = connection.GetConnection() as StringReader;
            datastore.AddColumnMetadata(new ColumnMetadata(0, "TextFile"));
            datastore.AddData(new object[] { stringReader.ReadToEnd() });
        }
    }
}
