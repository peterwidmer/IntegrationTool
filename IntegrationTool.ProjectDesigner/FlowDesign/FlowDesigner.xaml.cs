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
        public event EventHandler DesignerClicked;

        private Package package;

        public FlowDesigner(Package package, List<ModuleDescription> loadedModules, DesignerCanvasType designerCanvasType)
        {
            InitializeComponent();
            this.package = package;
            MyDesigner.ModuleDescriptions = loadedModules;
            MyDesigner.DesignerCanvasType = designerCanvasType;
            MyDesigner.Clicked += MyDesigner_Clicked;
            
            switch(designerCanvasType)
            {
                case DesignerCanvasType.MainFlow:
                    FlowHeader.Content = "Main Sequence";
                    break;

                case DesignerCanvasType.SubFlow:
                    FlowHeader.Content = "Sub-Sequence";
                    break;
            }
        }

        void MyDesigner_Clicked(object sender, EventArgs e)
        {
            if(DesignerClicked != null)
            {
                DesignerClicked(sender, e);
            }            
        }

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
