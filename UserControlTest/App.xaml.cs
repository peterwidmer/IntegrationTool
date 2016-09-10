using IntegrationTool.ApplicationCore;
using IntegrationTool.ProjectDesigner.MenuWindows;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UserControlTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Guid CRMCONNECTIONID = new Guid("4D3F2E27-71EC-4631-8E98-5915E99FCED2");
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //StartCrmConfigurationWindow();
            //StartStringTransformationWindow();
            StartFlowToolbox();
        }

        private void StartFlowToolbox()
        {
            ApplicationInitializer applicationInitializer = new ApplicationInitializer();
            FlowToolboxWindow flowToolboxWindow = new FlowToolboxWindow();
            flowToolboxWindow.content.Content = new IntegrationTool.ProjectDesigner.FlowDesign.FlowToolbox(applicationInitializer.ModuleLoader.Modules.Where(t=>t.Attributes.ModuleType == ModuleType.Step).ToList());
            flowToolboxWindow.ShowDialog();
        }

        private void StartStringTransformationWindow()
        {
            IDatastore dataObject = GetContactsDatastore();

            IntegrationTool.Module.StringTranformation.StringTransformationConfiguration stringTransformationConfiguration = new IntegrationTool.Module.StringTranformation.StringTransformationConfiguration();
            stringTransformationConfiguration.ConfigurationId = Guid.NewGuid();
            stringTransformationConfiguration.Name = "MyTransformation";
            stringTransformationConfiguration.Transformations.Add(new IntegrationTool.Module.StringTranformation.SDK.StringTransformationParameter() {TransformationType=IntegrationTool.Module.StringTranformation.SDK.Enums.StringTransformationType.Replace, Parameter1="Bla", Parameter2="bla2"});

            List<IntegrationTool.Module.StringTranformation.SDK.StringTransformationAttribute> transformationAttributes = IntegrationTool.Module.StringTranformation.SDK.Helpers.LoadAllTransformationClasses();

            IntegrationTool.Module.StringTranformation.ConfigurationWindow stringTransformationConfigWindow = new IntegrationTool.Module.StringTranformation.ConfigurationWindow(stringTransformationConfiguration, transformationAttributes, dataObject);

            IntegrationTool.ProjectDesigner.MenuWindows.ConfigurationWindow configWindow = GetConfigurationWindow(stringTransformationConfigWindow, true, stringTransformationConfiguration, ModuleType.Transformation, dataObject);
            configWindow.Closing += configWindow_Closing;
            configWindow.Show();
        }

        private void StartCrmConfigurationWindow()
        {
            IntegrationTool.Module.WriteToDynamicsCrm.WriteToDynamicsCrmConfiguration writeToCrmConfig = new IntegrationTool.Module.WriteToDynamicsCrm.WriteToDynamicsCrmConfiguration();
            writeToCrmConfig.EntityName = "account";
            writeToCrmConfig.ConfigurationId = Guid.NewGuid();
            writeToCrmConfig.SelectedConnectionConfigurationId = CRMCONNECTIONID;

            IDatastore dataObject = GetContactsDatastore();

            IntegrationTool.Module.WriteToDynamicsCrm.ConfigurationWindow writeToCrmConfigWindow = new IntegrationTool.Module.WriteToDynamicsCrm.ConfigurationWindow(writeToCrmConfig, dataObject);
            IntegrationTool.ProjectDesigner.MenuWindows.ConfigurationWindow configWindow = GetConfigurationWindow(writeToCrmConfigWindow, true, writeToCrmConfig, ModuleType.Target, dataObject);
            configWindow.Closing += configWindow_Closing;
            configWindow.Show();
        }

        void configWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IntegrationTool.ProjectDesigner.MenuWindows.ConfigurationWindow configWindow = sender as IntegrationTool.ProjectDesigner.MenuWindows.ConfigurationWindow;
            var config = configWindow.DataContext;
        }

        private IntegrationTool.ProjectDesigner.MenuWindows.ConfigurationWindow GetConfigurationWindow(UserControl configurationControl, bool requiresConnectionConfiguration, StepConfigurationBase configuration, ModuleType moduleType, IDatastore datastore )
        {
            ObservableCollection<ConnectionConfigurationBase> connections = new ObservableCollection<ConnectionConfigurationBase>();
            
            // Define Connections
            IntegrationTool.Module.ConnectToDynamicsCrm.ConnectToDynamicsCrmConfiguration connectToDynamicsCrmConfiguration = new IntegrationTool.Module.ConnectToDynamicsCrm.ConnectToDynamicsCrmConfiguration();
            connectToDynamicsCrmConfiguration.Name = "Crm Connection";
            connectToDynamicsCrmConfiguration.ConfigurationId = CRMCONNECTIONID;
            connectToDynamicsCrmConfiguration.ConnectionString = "Url=http://devserv/DataProc; Domain=DEV; Username=johntest; Password=12ab!12ab;";
            connections.Add(connectToDynamicsCrmConfiguration);

            ModuleDescription moduleDescription = new ModuleDescription()
            {
                Attributes = new ModuleAttributeBase()
                {
                    RequiresConnection = true,
                    ModuleType = ModuleType.Target
                }
            };
            
            ConfigurationWindowSettings configurationWindowSettings = new ConfigurationWindowSettings()
            {
                title = "Test CRM",
                titleImage = null,
                connections = connections,
                configurationControl  = configurationControl,
                moduleDescription = moduleDescription,
                configuration = configuration,
                datastore = datastore
            };

            ConfigurationWindow configWindow = new ConfigurationWindow(configurationWindowSettings);

            return configWindow;
        }

        private IDatastore GetContactsDatastore()
        {
            IDatastore dataObject = DataStoreFactory.GetDatastore();
            dataObject.AddColumn(new ColumnMetadata("FirstName"));
            dataObject.AddColumn(new ColumnMetadata("LastName"));
            dataObject.AddColumn(new ColumnMetadata("City"));
            dataObject.AddColumn(new ColumnMetadata("ID"));

            dataObject.AddData(new object[] { "Peter", "Widmer", "Wettingen", 1001 });
            dataObject.AddData(new object[] { "Joachim", "Suter", "Dättwil", 1002 });

            return dataObject;
        }
    }
}
