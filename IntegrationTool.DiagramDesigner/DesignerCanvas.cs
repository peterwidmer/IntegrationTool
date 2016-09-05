using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using System.Windows.Shapes;
using IntegrationTool.SDK;

namespace IntegrationTool.DiagramDesigner
{
    public enum DesignerCanvasType
    {
        MainFlow, SubFlow
    }

    public class ItemsPastedEventArgs : EventArgs
    {
        public Dictionary<Guid, Guid> MappingOldToNewIDs { get; set; }
    }

    public partial class DesignerCanvas : Canvas
    {
        public static readonly RoutedEvent MagnifyDoubleClickEvent = EventManager.RegisterRoutedEvent("MagnifyDoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DesignerItem));
        public event RoutedEventHandler MagnifyDoubleClick
        {
            add { AddHandler(MagnifyDoubleClickEvent, value); }
            remove { RemoveHandler(MagnifyDoubleClickEvent, value); }
        }

        public void RaiseMagnifyDoubleClick(DesignerItem designerItem)
        {
            RaiseEvent(new RoutedEventArgs(DesignerCanvas.MagnifyDoubleClickEvent, designerItem));
        }

        public event EventHandler Clicked;

        public event EventHandler OnDeleteCurrentSelection;
        public event EventHandler OnCopyCurrentSelection;

        public delegate void OnPastedCurrentSelectionEventHandler(object sender, ItemsPastedEventArgs e);
        public event OnPastedCurrentSelectionEventHandler OnPastedCurrentSelection;

        public delegate void PasteEnabledEventHandlerobject(object sender, CanExecuteRoutedEventArgs e);
        public event PasteEnabledEventHandlerobject IsPasteEnabled;

        public DesignerCanvasType DesignerCanvasType { get; set; }

        private Point? rubberbandSelectionStartPoint = null;

        private SelectionService selectionService;
        public SelectionService SelectionService
        {
            get
            {
                if (selectionService == null)
                    selectionService = new SelectionService(this);

                return selectionService;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                // in case that this click is the start of a 
                // drag operation we cache the start point
                this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(this));

                // if you click directly on the canvas all 
                // selected items are 'de-selected'
                SelectionService.ClearSelection();
                Focus();
                if(Clicked != null)
                {
                    Clicked(this, new EventArgs());
                }
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // if mouse button is not pressed we have no drag operation, ...
            if (e.LeftButton != MouseButtonState.Pressed)
                this.rubberbandSelectionStartPoint = null;

            // ... but if mouse button is pressed and start
            // point value is set we do have one
            if (this.rubberbandSelectionStartPoint.HasValue)
            {
                // create rubberband adorner
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, rubberbandSelectionStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }
            }
            e.Handled = true;
        }

        public System.Windows.Shapes.Path GetContent(string itemStyle, string itemControlTemplateStyle, ModuleDescription module)
        {
            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path { ToolTip = module.Attributes.DisplayName };
            path.Style = (Style)FindResource(itemStyle);
            var processElement = new FrameworkElementFactory(typeof(System.Windows.Shapes.Path));
            processElement.SetValue(StyleProperty, (Style)FindResource(itemControlTemplateStyle));
            ControlTemplate controlTemplate = new ControlTemplate();
            controlTemplate.VisualTree = processElement;
            path.SetValue(DesignerItem.DragThumbTemplateProperty, controlTemplate);
            path.Tag = module;
            return path;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DragObject dragObject = e.Data.GetData(typeof(DragObject)) as DragObject;
            if (dragObject != null)
            {
                DesignerItem newItem = null;
                Object content = GetContent("Process", "Process_DragThumb", dragObject.ModuleDescription);
                if (content != null)
                {
                    newItem = new DesignerItem();
                    newItem.Content = content;
                    newItem.ModuleDescription = dragObject.ModuleDescription;
                    
                    // Next to do - save and load moduledescription on designerItems!
                    Point position = e.GetPosition(this);

                    if (dragObject.DesiredSize.HasValue)
                    {
                        Size desiredSize = dragObject.DesiredSize.Value;
                        newItem.Width = desiredSize.Width;
                        newItem.Height = desiredSize.Height;

                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X - newItem.Width / 2));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y - newItem.Height / 2));
                    }
                    else
                    {
                        DesignerCanvas.SetLeft(newItem, Math.Max(0, position.X));
                        DesignerCanvas.SetTop(newItem, Math.Max(0, position.Y));
                    }

                    Canvas.SetZIndex(newItem, this.Children.Count);
                    this.Children.Add(newItem);
                    SetDesignerItemConnectorDecoratorTemplate(newItem);

                    //update selection
                    this.SelectionService.SelectItem(newItem);
                    newItem.Focus();
                }

                e.Handled = true;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size size = new Size();

            foreach (UIElement element in this.InternalChildren)
            {
                double left = Canvas.GetLeft(element);
                double top = Canvas.GetTop(element);
                left = double.IsNaN(left) ? 0 : left;
                top = double.IsNaN(top) ? 0 : top;

                //measure desired size for each child
                element.Measure(constraint);

                Size desiredSize = element.DesiredSize;
                if (!double.IsNaN(desiredSize.Width) && !double.IsNaN(desiredSize.Height))
                {
                    size.Width = Math.Max(size.Width, left + desiredSize.Width);
                    size.Height = Math.Max(size.Height, top + desiredSize.Height);
                }
            }
            // add margin 
            size.Width += 10;
            size.Height += 10;
            return size;
        }

        private void SetConnectorDecoratorTemplate(DesignerItem item)
        {
            if (item.Content is UIElement)
            {
                ControlTemplate template = DesignerItem.GetConnectorDecoratorTemplate(item.Content as UIElement);
                SetConnectorDecoratorTemplate(item, template);
            }
        }

        private void SetConnectorDecoratorTemplate(DesignerItem item, ControlTemplate template)
        {
            if (item.ApplyTemplate())
            {
                Control decorator = item.Template.FindName("PART_ConnectorDecorator", item) as Control;
                if (decorator != null && template != null)
                {
                    decorator.Template = template;
                    decorator.ApplyTemplate();
                }
            }
        }
    }
}
