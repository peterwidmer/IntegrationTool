using IntegrationTool.DiagramDesigner;
using IntegrationTool.ApplicationCore;
using IntegrationTool.ApplicationCore.Serialization;
using IntegrationTool.Flowmanagement;
using IntegrationTool.ProjectDesigner.Classes;
using IntegrationTool.ProjectDesigner.MenuWindows;
using IntegrationTool.ProjectDesigner.Screens.UserControls;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using IntegrationTool.SDK.Diagram;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IntegrationTool.ProjectDesigner.FlowDesign;
using System.Xml.Linq;
using System.Globalization;
using IntegrationTool.SDK.Data;
using IntegrationTool.SDK.Exceptions;

namespace IntegrationTool.ProjectDesigner.Screens
{
    /// <summary>
    /// Interaction logic for PackageOverview.xaml
    /// </summary>
    public partial class PackageOverview : UserControl
    {
        
        public event EventHandler BackButtonClicked;
        public event EventHandler ProgressReport;

        private StepCopy StepCopy { get; set; }

        public Package Package { get; set; }
        private ModuleLoader moduleLoader;
        public ObservableCollection<ConnectionConfigurationBase> Connections { get; set; }
        private DesignerItem doubleClickedMainflowDesignerItem { get; set; }

        private FlowDesign.FlowDesigner mainFlowDesigner;

        public PackageOverview(ModuleLoader moduleLoader, ObservableCollection<ConnectionConfigurationBase> connections, Package package)
        {
            InitializeComponent();
            InitializeCommands();

            this.moduleLoader = moduleLoader;
            this.Connections = connections;
            this.DataContext = this.Package = package;

            InitializePackageOverview();
        }

        public void InitializePackageOverview()
        {
            // Initialize Mainflow Designer            
            mainFlowDesigner = new FlowDesign.FlowDesigner(this.Package, moduleLoader.Modules, DesignerCanvasType.MainFlow);
            mainFlowDesigner.MyDesigner.LoadDiagramFromXElement(this.Package.Diagram.Diagram);
            mainFlowDesigner.DesignerItemClick += mainFlowDesigner_DesignerItemClick;
            mainFlowDesigner.DesignerItemDoubleClick += mainFlowDesigner_DesignerItemDoubleClick;
            mainFlowDesigner.DesignerClicked += designerClicked;
            mainFlowDesigner.MyDesigner.OnDeleteCurrentSelection += MyDesigner_OnDeleteCurrentSelection;
            mainFlowDesigner.MyDesigner.OnCopyCurrentSelection += MyDesigner_OnCopyCurrentSelection;
            mainFlowDesigner.MyDesigner.OnPastedCurrentSelection += MyDesigner_OnPastedCurrentSelection;
            mainFlowDesigner.MyDesigner.IsPasteEnabled += MyDesigner_IsPasteEnabled;
            this.mainFlowContent.Content = mainFlowDesigner;
            InitializeSubFlowDesigner();            
        }

        void MyDesigner_IsPasteEnabled(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = 
                this.StepCopy != null && 
                ((DesignerCanvas)sender).DesignerCanvasType == this.StepCopy.Type;
        }

        void MyDesigner_OnPastedCurrentSelection(object sender, ItemsPastedEventArgs e)
        {
            List<SerializedDiagram> copiedDiagrams = ConfigurationSerializer.DeserializeObject(
                                                    this.StepCopy.Diagrams,
                                                    typeof(List<SerializedDiagram>),
                                                    moduleLoader.GetModuleTypeList()
                                                    ) as List<SerializedDiagram>;

            foreach (SerializedDiagram diagram in copiedDiagrams)
            {
                foreach (var designerItem in diagram.Diagram.Element("DesignerItems").Elements("DesignerItem"))
                {
                    Guid oldGuid = new Guid(designerItem.Element("ID").Value);
                    Guid newGuid = Guid.NewGuid();
                    e.MappingOldToNewIDs.Add(oldGuid, newGuid);
                    designerItem.Element("ID").Value = newGuid.ToString();
                }

                foreach (var connection in diagram.Diagram.Element("Connections").Elements("Connection"))
                {
                    Guid oldSourceID = new Guid(connection.Element("SourceID").Value);
                    connection.Element("SourceID").Value = e.MappingOldToNewIDs[oldSourceID].ToString();

                    Guid oldSinkID = new Guid(connection.Element("SinkID").Value);
                    connection.Element("SinkID").Value = e.MappingOldToNewIDs[oldSinkID].ToString();
                }

                diagram.ParentItemId = e.MappingOldToNewIDs[diagram.ParentItemId];

                this.Package.SubDiagrams.Add(diagram);
            }

            List<ConfigurationBase> copiedConfigurations = ConfigurationSerializer.DeserializeObject(
                                                    this.StepCopy.Configurations,
                                                    typeof(List<ConfigurationBase>),
                                                    moduleLoader.GetModuleTypeList()
                                                    ) as List<ConfigurationBase>;

            foreach (ConfigurationBase configuration in copiedConfigurations)
            {
                configuration.ConfigurationId = e.MappingOldToNewIDs[configuration.ConfigurationId];
                this.Package.Configurations.Add(configuration);
            }
        }

        void MyDesigner_OnCopyCurrentSelection(object sender, EventArgs e)
        {
            this.StepCopy = new StepCopy();
            this.StepCopy.Type = ((DesignerCanvas)sender).DesignerCanvasType;
            List<ConfigurationBase> configurationsToCopy = new List<ConfigurationBase>();
            List<SerializedDiagram> diagramsToCopy = new List<SerializedDiagram>();

            DesignerCanvas designerCanvas = sender as DesignerCanvas;
            foreach (DesignerItem designerItem in designerCanvas.SelectionService.CurrentSelection.OfType<DesignerItem>())
            {
                // Copy sub-configurations (diagram and configurations)
                var subDiagram = this.Package.SubDiagrams.Where(t => t.ParentItemId == designerItem.ID).FirstOrDefault();
                if (subDiagram != null)
                {
                    diagramsToCopy.Add(subDiagram);
                    List<DesignerItem> subdiagramDesignerItems = DesignerCanvas.LoadDiagramDesignerItems(subDiagram.Diagram, this.moduleLoader.Modules);
                    foreach (DesignerItem subdiagramDesignerItem in subdiagramDesignerItems)
                    {
                        var subconfiguration = this.Package.Configurations.Where(t => t.ConfigurationId == subdiagramDesignerItem.ID).FirstOrDefault();
                        if (subconfiguration != null)
                        {
                            configurationsToCopy.Add(subconfiguration);
                        }
                    }
                }

                // Copy main configuration
                var configuration = this.Package.Configurations.Where(t => t.ConfigurationId == designerItem.ID).FirstOrDefault();
                if (configuration != null)
                {
                    configurationsToCopy.Add(configuration);
                }
            }

            this.StepCopy.Configurations = ConfigurationSerializer.SerializeObject(configurationsToCopy, moduleLoader.GetModuleTypeList());
            this.StepCopy.Diagrams = ConfigurationSerializer.SerializeObject(diagramsToCopy, moduleLoader.GetModuleTypeList());
        }

        void MyDesigner_OnDeleteCurrentSelection(object sender, EventArgs e)
        {
            DesignerCanvas designerCanvas = sender as DesignerCanvas;
            foreach(DesignerItem designerItem in designerCanvas.SelectionService.CurrentSelection.OfType<DesignerItem>())
            {                
                // Remove sub-configurations (diagram and configurations)
                var subDiagram = this.Package.SubDiagrams.Where(t => t.ParentItemId == designerItem.ID).FirstOrDefault();
                if (subDiagram != null)
                {
                    List<DesignerItem> subdiagramDesignerItems = DesignerCanvas.LoadDiagramDesignerItems(subDiagram.Diagram, this.moduleLoader.Modules);
                    foreach(DesignerItem subdiagramDesignerItem in subdiagramDesignerItems)
                    {
                        this.Package.Configurations.RemoveAll(t => t.ConfigurationId == subdiagramDesignerItem.ID);
                    }
                }
                this.Package.SubDiagrams.RemoveAll(t => t.ParentItemId == designerItem.ID);

                // Remove main-configuration
                this.Package.Configurations.RemoveAll(t => t.ConfigurationId == designerItem.ID);
            }
        }

        void designerClicked(object sender, EventArgs e)
        {
            propertiesRow.Height = new GridLength(0, GridUnitType.Pixel);
            PropertiesContentControl.Content = null;
            propertiesSplitter.Visibility = System.Windows.Visibility.Hidden;
        }

        void mainFlowDesigner_DesignerItemClick(object sender, EventArgs e)
        {
            StepProperty stepProperty = new StepProperty();
            stepProperty.DesignerItem = ((RoutedEventArgs)e).OriginalSource as DesignerItem;
            stepProperty.Configuration = GetStepConfiguration(stepProperty.DesignerItem.ID, stepProperty.DesignerItem.ModuleDescription, this.Package);

            propertiesRow.Height = new GridLength(100, GridUnitType.Star);
            this.PropertiesContentControl.Content = new PropertiesControl(stepProperty);
            this.propertiesSplitter.Visibility = System.Windows.Visibility.Visible;
        }

        private void InitializeSubFlowDesigner()
        {
            // Initialize Subflow Designer
            var subFlowDesigner = new FlowDesign.FlowDesigner(this.Package, moduleLoader.Modules, DesignerCanvasType.SubFlow);
            subFlowDesigner.DesignerItemDoubleClick += subFlowDesigner_DesignerItemDoubleClick;
            subFlowDesigner.SubflowMagnifyIconDoubleClick += subFlowDesigner_SubflowMagnifyIconDoubleClick;
            subFlowDesigner.MyDesigner.OnDeleteCurrentSelection += MyDesigner_OnDeleteCurrentSelection;
            subFlowDesigner.MyDesigner.OnCopyCurrentSelection += MyDesigner_OnCopyCurrentSelection;
            subFlowDesigner.MyDesigner.OnPastedCurrentSelection += MyDesigner_OnPastedCurrentSelection;
            subFlowDesigner.MyDesigner.IsPasteEnabled += MyDesigner_IsPasteEnabled;
            propertiesRow.Height = new GridLength(0);
            this.subFlowContent.Content = subFlowDesigner;
        }

        void subFlowDesigner_SubflowMagnifyIconDoubleClick(object sender, EventArgs e)
        {
            try
            {
                DesignerItem designerItem = ((RoutedEventArgs)e).OriginalSource as DesignerItem;
                SubFlowExecution subFlowExecution = GetSubflowExecution();
                var dataStores = subFlowExecution.GetDataObjectForDesignerItem(designerItem.ID, true,true, null);
                DataPreviewWindow dataPreviewWindow = new DataPreviewWindow(dataStores.First());
                dataPreviewWindow.Show();
            }
            catch (Exception ex)
            {
                HandleWindowOpenExceptions(ex);
            }
        }

        private StepConfigurationBase GetStepConfiguration(Guid designerItemId, ModuleDescription itemModuleDescription, Package package)
        {
            StepConfigurationBase configuration = package.Configurations.Where(t => t.ConfigurationId == designerItemId).FirstOrDefault() as StepConfigurationBase;
            if (configuration == null)
            {
                configuration = Activator.CreateInstance(itemModuleDescription.Attributes.ConfigurationType) as StepConfigurationBase;
                configuration.ConfigurationId = designerItemId;
                package.Configurations.Add(configuration);
            }

            return configuration;
        }

        void subFlowDesigner_DesignerItemDoubleClick(object sender, EventArgs e)
        {
            try
            {
                DesignerItem designerItem = sender as DesignerItem;

                StepConfigurationBase configuration = GetStepConfiguration(designerItem.ID, designerItem.ModuleDescription, this.Package);

                SaveSubflowConfiguration();

                SubFlowExecution subFlowExecution = GetSubflowExecution();

                var dataStores = designerItem.ModuleDescription.Attributes.ModuleType == ModuleType.Source ?
                    new List<IDatastore>() { new DummyDataStore() } :
                    subFlowExecution.GetDataObjectForDesignerItem(designerItem.ID, false, true, null);

                ConfigurationWindowSettings configurationWindowSettings = ConfigurationWindowSettings.Get(designerItem, configuration, this.moduleLoader, dataStores, Connections);
                ShowConfiguationWindow(configurationWindowSettings);
            }
            catch(Exception ex)
            {
                HandleWindowOpenExceptions(ex);
            }
        }

        public void HandleWindowOpenExceptions(Exception ex)
        {
            string errorMessage = ex is InfoException ? ex.Message : ex.ToString();
            MessageBox.Show(errorMessage);
        }

        void ShowConfiguationWindow(ConfigurationWindowSettings configurationWindowSettings)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow(configurationWindowSettings);
            configurationWindow.Closing += configurationWindow_Closing;
            configurationWindow.ShowDialog();
        }        

        void configurationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConfigurationWindow configurationWindow = sender as ConfigurationWindow;
            ConfigurationWindowSettings settings = configurationWindow.ConfigurationWindowSettings;
            if(configurationWindow.Status == Classes.WindowResult.Canceled)
            {
                Type configurationType = settings.configuration.GetType();
                object revertedConfiguration = ConfigurationSerializer.DeserializeObject(
                                                    settings.originalConfiguration,
                                                    configurationType,
                                                    moduleLoader.GetModuleTypeList()
                                                    );

                int indexConfig = Package.Configurations.IndexOf(settings.configuration);
                Package.Configurations[indexConfig] = revertedConfiguration as StepConfigurationBase;
            }            
        }

        public SubFlowExecution GetSubflowExecution()
        {
            var objectResolver = new ObjectResolver(Package.Configurations.OfType<StepConfigurationBase>().ToList(), Connections);
            var subDiagram = this.Package.SubDiagrams.Where(t => t.ParentItemId == doubleClickedMainflowDesignerItem.ID).FirstOrDefault();
            var diagramDeserializer = new SDK.Diagram.DiagramDeserializer(moduleLoader.Modules, subDiagram.Diagram);
            var flowGraph = new FlowGraph(diagramDeserializer.DesignerItems, diagramDeserializer.Connections);
            var subFlowExecution = new SubFlowExecution(null, null, objectResolver, flowGraph);

            return subFlowExecution;
        }

        void mainFlowDesigner_DesignerItemDoubleClick(object sender, EventArgs e)
        {
            try
            {
                doubleClickedMainflowDesignerItem = sender as DesignerItem;
                if (doubleClickedMainflowDesignerItem.ModuleDescription.Attributes.ContainsSubConfiguration)
                {
                    packageDesignerTabControl.SelectedIndex = 1;
                }
                else
                {
                    StepConfigurationBase configuration = GetStepConfiguration(doubleClickedMainflowDesignerItem.ID, doubleClickedMainflowDesignerItem.ModuleDescription, this.Package);
                    ConfigurationWindowSettings configurationWindowSettings = ConfigurationWindowSettings.Get(doubleClickedMainflowDesignerItem, configuration, this.moduleLoader, null, Connections);
                    ShowConfiguationWindow(configurationWindowSettings);
                }
            }
            catch (Exception ex)
            {
                HandleWindowOpenExceptions(ex);
            }
        }

        public void SaveDiagram()
        {
            var mainFlowDesigner = this.mainFlowContent.Content as FlowDesign.FlowDesigner;
            if(mainFlowDesigner != null)
            {
                this.Package.Diagram.Diagram = mainFlowDesigner.MyDesigner.SaveDiagramToXElement();
            }

            SaveSubflowConfiguration();
        }

        private void packageDesignerTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(packageDesignerTabControl.SelectedIndex)
            {
                case 0:
                    this.subFlowTab.Visibility = System.Windows.Visibility.Hidden;
                    subFlowContent.Visibility = Visibility.Hidden;
                    SaveSubflowConfiguration();
                    this.doubleClickedMainflowDesignerItem = null;
                    this.ToolboxContent.Content = new ToolboxMainflow(moduleLoader.Modules);
                    break;

                case 1:
                    InitializeSubFlowDesigner();
                    LoadSubflowConfiguration();
                    this.subFlowTab.Visibility = System.Windows.Visibility.Visible;
                    subFlowContent.Visibility = Visibility.Visible;
                    this.ToolboxContent.Content = new ToolboxSubflow(moduleLoader.Modules);
                    break;
            }
        }

        private void SaveSubflowConfiguration()
        {
            if(doubleClickedMainflowDesignerItem != null)
            {
                SerializedDiagram subDiagram = this.Package.SubDiagrams.Where(t => t.ParentItemId == doubleClickedMainflowDesignerItem.ID).FirstOrDefault();

                var subFlowDesigner = this.subFlowContent.Content as FlowDesign.FlowDesigner;
                if (subFlowDesigner != null && subDiagram != null)
                {
                    subDiagram.Diagram = subFlowDesigner.MyDesigner.SaveDiagramToXElement();
                }
            }
        }

        private void LoadSubflowConfiguration()
        {
            if (doubleClickedMainflowDesignerItem != null)
            {
                SerializedDiagram subDiagram = this.Package.SubDiagrams.Where(t => t.ParentItemId == doubleClickedMainflowDesignerItem.ID).FirstOrDefault();
                if (subDiagram == null)
                {
                    subDiagram = new SerializedDiagram();
                    subDiagram.ParentItemId = doubleClickedMainflowDesignerItem.ID;
                    this.Package.SubDiagrams.Add(subDiagram);
                }
                else
                {
                    var subFlowDesigner = this.subFlowContent.Content as FlowDesign.FlowDesigner;
                    if (subFlowDesigner != null)
                    {
                        subFlowDesigner.MyDesigner.LoadDiagramFromXElement(subDiagram.Diagram);
                    }
                }
            }
        }       
        
    }
}
