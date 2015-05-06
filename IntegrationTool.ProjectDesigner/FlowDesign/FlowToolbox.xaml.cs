using IntegrationTool.DiagramDesigner;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntegrationTool.ProjectDesigner.FlowDesign
{
    /// <summary>
    /// Interaction logic for FlowToolbox.xaml
    /// </summary>
    public partial class FlowToolbox : UserControl
    {
        Point _dragStartPoint;
        TreeViewItem _dragTreeviewItem;
        bool IsDragging;

        private List<ModuleDescription> modules;
        private Dictionary<string, TreeViewItem> moduleGroups { get; set; }

        public FlowToolbox(List<ModuleDescription> modules)
        {
            InitializeComponent();
            moduleGroups = new Dictionary<string, TreeViewItem>();
            this.modules = modules;

            InitializeFlowToolbox();
        }

        private void InitializeFlowToolbox()
        {
            foreach (var module in this.modules.OrderBy(t=> t.Attributes.GroupName))
            {
                string groupName = module.Attributes.GroupName.ToString();
                if (!moduleGroups.ContainsKey(groupName))
                    AddGroupToToolbox(groupName);

                TreeViewItem trvTool = CreateTreeViewItem(module.Attributes.DisplayName, module.ModuleType.Assembly);
                trvTool.Tag = module;

                trvTool.PreviewMouseLeftButtonDown += trvTool_PreviewMouseLeftButtonDown;
                trvTool.PreviewMouseMove += trvTool_PreviewMouseMove;
                moduleGroups[groupName].Items.Add(trvTool);
            }
        }

        void trvTool_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsDragging)
            {
                Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }

        void trvTool_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            if (sender as TreeViewItem != null)
                _dragTreeviewItem = sender as TreeViewItem;
        }

        private void StartDrag(MouseEventArgs e)
        {
            IsDragging = true;
            DragObject dragObject = new DragObject();
            dragObject.ModuleDescription = _dragTreeviewItem.Tag as ModuleDescription;
            dragObject.DesiredSize = new Size(160, 40);

            DragDropEffects de = DragDrop.DoDragDrop(_dragTreeviewItem, dragObject, DragDropEffects.Move);
            IsDragging = false;
        }

        private void AddGroupToToolbox(string GroupName)
        {
            TreeViewItem trvItem = trvItem = new TreeViewItem() { Header = GroupName, Tag = GroupName };
            trvItem.IsExpanded = true;
            moduleGroups.Add(GroupName, trvItem);
            toolBoxMenu.Items.Add(trvItem);
        }

        private TreeViewItem CreateTreeViewItem(string header, Assembly assembly)
        {
            TreeViewItem child = new TreeViewItem();
            StackPanel pan = new StackPanel();

            pan.Orientation = Orientation.Horizontal;

            Image image = RenderIcon(assembly);
            image.Height = 16;
            image.Width = 16;
            pan.Children.Add(image);

            pan.Children.Add(new TextBlock(new Run("  " + header)));
            child.Header = pan;
            return child;

        }

        public Image RenderIcon(Assembly assembly)
        {
            string strIcon = string.Empty;
            string[] resources = assembly.GetManifestResourceNames();
            foreach (string resource in resources)
            {
                if (resource.ToLower().EndsWith(".icon.xml"))
                {
                    using (System.IO.Stream stream = assembly.GetManifestResourceStream(resource))
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        strIcon = reader.ReadToEnd();
                    }

                    System.Windows.Controls.Image db = (System.Windows.Controls.Image)System.Windows.Markup.XamlReader.Load(System.Xml.XmlReader.Create(new System.IO.StringReader(strIcon)));

                    return db;
                }
            }

            throw new Exception("Could not find Icon.txt in Assembly " + assembly.FullName);
        }
    }
}
