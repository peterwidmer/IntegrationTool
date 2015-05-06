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
    /// Interaction logic for ToolboxMainflow.xaml
    /// </summary>
    public partial class ToolboxMainflow : UserControl
    {
        public ToolboxMainflow(List<ModuleDescription> loadedModules)
        {
            InitializeComponent();
            MainToolbox.Content = new FlowToolbox(loadedModules.Where(t=>t.Attributes.ModuleType == ModuleType.Step).ToList());
        }
    }
}
