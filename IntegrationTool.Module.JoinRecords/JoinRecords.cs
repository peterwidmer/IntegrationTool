using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.JoinRecords
{
    [TransformationModuleAttribute(
        Name = "JoinRecords",
        DisplayName = "Join Records",
        ModuleType = ModuleType.Transformation,
        GroupName = ModuleGroup.Transformation,
        ConfigurationType = typeof(JoinRecordsConfiguration),
        RequiresConnection = false)]
    public partial class JoinRecords : IModule, IDataMerge
    {
        public JoinRecordsConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as JoinRecordsConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            throw new NotImplementedException();
        }


        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore datastoreOne, IDatastore datastoreTwo)
        {
            var joinRecordsConfiguration = configurationBase as JoinRecordsConfiguration;
            return new ConfigurationWindow(joinRecordsConfiguration, datastoreOne, datastoreTwo);            
        }

        public bool HasDatastoreOneToBeSetAsTheSourceTable(JoinRecordsConfiguration joinRecordsConfiguration, IDatastore datastoreOne, IDatastore datastoreTwo)
        {
            if(joinRecordsConfiguration.JoinMapping.Count == 0)
            {
                return datastoreOne.Count > datastoreTwo.Count;
            }
            else
            {
                return IsDatastoreOneConfiguredInTheSourceMapping(joinRecordsConfiguration, datastoreOne, datastoreTwo);
            }
        }

        public bool IsDatastoreOneConfiguredInTheSourceMapping(JoinRecordsConfiguration joinRecordsConfiguration, IDatastore datastoreOne, IDatastore datastoreTwo)
        {
            foreach(var mapping in joinRecordsConfiguration.JoinMapping)
            {
                if(!datastoreOne.Metadata.ContainsColumn(mapping.Source) || !datastoreTwo.Metadata.ContainsColumn(mapping.Target))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
