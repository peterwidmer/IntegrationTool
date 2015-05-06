using IntegrationTool.DiagramDesigner;
using IntegrationTool.ProjectDesigner.Classes;
using IntegrationTool.ProjectDesigner.Screens.UserControls;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
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

namespace IntegrationTool.ProjectDesigner.FlowDesign
{
    /// <summary>
    /// Interaction logic for FlowDesigner.xaml
    /// </summary>
    public partial class FlowDesigner : UserControl
    {
        private Dictionary<string, Expander> expanders = new Dictionary<string, Expander>();

        public event EventHandler DesignerItemClick;
        public event EventHandler DesignerItemDoubleClick;
        public event EventHandler SubflowMagnifyIconDoubleClick;        

        private Package package;

        public FlowDesigner(Package package, UserControl toolboxControl, List<ModuleDescription> loadedModules, DesignerCanvasType designerCanvasType)
        {
            InitializeComponent();
            this.package = package;
            MyDesigner.ModuleDescriptions = loadedModules;
            MyDesigner.DesignerCanvasType = designerCanvasType;
            MyDesigner.Clicked += MyDesigner_Clicked;
            this.ToolboxContent.Content = toolboxControl;
            //InitializeModules(loadedModules, typesToShow);
        }

        void MyDesigner_Clicked(object sender, EventArgs e)
        {
            propertiesRow.Height = new GridLength(0, GridUnitType.Pixel);
            PropertiesContentControl.Content = null;
            propertiesSplitter.Visibility = System.Windows.Visibility.Hidden;
        }

        //private void InitializeModules(List<ModuleDescription> loadedModules, List<ModuleType> typesToShow)
        //{
        //    foreach (ModuleType moduleType in typesToShow)
        //    {
        //        Expander toolboxExpander = new Expander() { Header = moduleType.ToString(), Name = moduleType.ToString(), IsExpanded = true };
        //        toolboxExpander.Content = new Toolbox() { ItemSize = new Size(60, 50), SnapsToDevicePixels = true };
        //        expanders.Add(toolboxExpander.Name, toolboxExpander);
        //        ToolboxPanel.Children.Add(toolboxExpander);
        //    }

        //    foreach (ModuleDescription module in loadedModules.Where(t => typesToShow.Contains(t.Attributes.ModuleType)))
        //    {
        //        Toolbox toolbox = (Toolbox)expanders[module.Attributes.ModuleType.ToString()].Content;

        //        toolbox.AddToolboxItem("Process", "Process_DragThumb", module);
        //    }
        //}

        private void DesignerItem_DoubleClick(object sender, RoutedEventArgs e)
        {
            if(DesignerItemDoubleClick != null)
            {
                DesignerItemDoubleClick(e.OriginalSource, e);
            }
        }
        private void Magnify_DoubleClick(object sender, RoutedEventArgs e)
        {
            if(SubflowMagnifyIconDoubleClick != null)
            {
                SubflowMagnifyIconDoubleClick(sender, e);
            }
        }

        private void DesignerItem_Click(object sender, RoutedEventArgs e)
        {
            if(DesignerItemClick != null)
            {
                DesignerItemClick(sender, e);
            }
        }

        
    }


}
