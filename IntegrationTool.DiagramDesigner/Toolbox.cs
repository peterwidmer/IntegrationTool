using IntegrationTool.SDK;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace IntegrationTool.DiagramDesigner
{
    // Implements ItemsControl for ToolboxItems    
    public class Toolbox : ItemsControl
    {
        // Defines the ItemHeight and ItemWidth properties of
        // the WrapPanel used for this Toolbox
        public Size ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }
        private Size itemSize = new Size(50, 50);

        // Creates or identifies the element that is used to display the given item.        
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ToolboxItem();
        }

        // Determines if the specified item is (or is eligible to be) its own container.        
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }

        public void AddToolboxItem(string itemStyle, string itemControlTemplateStyle, ModuleDescription module)
        {
            Path path = new Path { ToolTip = module.Attributes.DisplayName };
            path.Style = (Style)FindResource(itemStyle);
            var processElement = new FrameworkElementFactory(typeof(Path));
            processElement.SetValue(StyleProperty, (Style)FindResource(itemControlTemplateStyle));
            ControlTemplate controlTemplate = new ControlTemplate();
            controlTemplate.VisualTree = processElement;
            path.SetValue(DesignerItem.DragThumbTemplateProperty, controlTemplate);
            path.Tag = module;
            this.Items.Add(path);
        }
    }
}
