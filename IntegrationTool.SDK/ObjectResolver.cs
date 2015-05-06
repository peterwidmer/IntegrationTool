using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.SDK
{
    public class ObjectResolver
    {
        private List<StepConfigurationBase> configurations;
        private ObservableCollection<ConnectionConfigurationBase> connectionConfigurations;

        public ObjectResolver(List<StepConfigurationBase> configurations, ObservableCollection<ConnectionConfigurationBase> connectionConfigurations)
        {
            this.configurations = configurations;
            this.connectionConfigurations = connectionConfigurations;
        }

        public IConnection GetConnection(Guid stepItemId)
        {
            var targetItemConfiguration = LoadItemConfiguration(stepItemId);

            ConnectionConfigurationBase connectionConfiguration = connectionConfigurations.Where(t => t.ConfigurationId == targetItemConfiguration.SelectedConnectionConfigurationId).FirstOrDefault();
            if(connectionConfiguration == null)
            {
                return null;
            }
            IModule connection = Activator.CreateInstance(connectionConfiguration.ModuleDescription.ModuleType) as IModule;
            connection.SetConfiguration(connectionConfiguration);

            return connection as IConnection;
        }

        public IModule GetModule(Guid stepItemId, Type moduleType)
        {
            var configuration = LoadItemConfiguration(stepItemId);

            return GetModule(moduleType, configuration);
        }

        public StepConfigurationBase LoadItemConfiguration(Guid itemId)
        {
            var itemConfiguration = this.configurations.Where(t => t.ConfigurationId == itemId).FirstOrDefault();
            if (itemConfiguration == null)
            {
                throw new Exception("Step does not contain a configuration");
            }

            return itemConfiguration;
        }

        public IModule GetModule(Type moduleType, StepConfigurationBase configuration)
        {
            IModule module = Activator.CreateInstance(moduleType) as IModule;
            module.SetConfiguration(configuration);

            return module;
        }
    }
}
