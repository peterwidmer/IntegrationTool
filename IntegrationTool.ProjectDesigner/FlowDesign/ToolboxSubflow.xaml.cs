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
    /// Interaction logic for ToolboxSubflow.xaml
    /// </summary>
    public partial class ToolboxSubflow : UserControl
    {
        public ToolboxSubflow(List<ModuleDescription> loadedModules)
        {
            InitializeComponent();
            SourceToolbox.Content = new FlowToolbox(loadedModules.Where(t => t.Attributes.ModuleType == ModuleType.Source).ToList());
            TransformationToolbox.Content = new FlowToolbox(loadedModules.Where(t => t.Attributes.ModuleType == ModuleType.Transformation).ToList());
            TargetToolbox.Content = new FlowToolbox(loadedModules.Where(t => t.Attributes.ModuleType == ModuleType.Target).ToList());
        }
    }
}
