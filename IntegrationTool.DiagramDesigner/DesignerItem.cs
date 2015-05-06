using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IntegrationTool.DiagramDesigner.Controls;
using System.Windows.Shapes;
using System.ComponentModel;
using IntegrationTool.SDK;

namespace IntegrationTool.DiagramDesigner
{
    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable
    {
        #region ID
        private Guid id;
        public Guid ID
        {
            get { return id; }
        }
        #endregion

        public BackgroundWorker BackgroundWorker { get; set; }

        public static readonly RoutedEvent DoubleClickEvent = EventManager.RegisterRoutedEvent("DoubleClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DesignerItem));
        public event RoutedEventHandler DoubleClick
        {
            add { AddHandler(DoubleClickEvent, value); }
            remove { RemoveHandler(DoubleClickEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DesignerItem));
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        private ModuleDescription moduleDescription;
        public ModuleDescription ModuleDescription
        {
            get
            {
                return moduleDescription;
            }
            set
            {
                ItemImage = IntegrationTool.SDK.Diagram.IconLoader.GetFromAssembly(value.ModuleType.Assembly, "Icon.xml");
                moduleDescription = value;
            }
        }

        #region ItemImage

        public Image ItemImage
        {
            get { return (Image)GetValue(ItemImageProperty); }
            set { SetValue(ItemImageProperty, value); }
        }
        public static readonly DependencyProperty ItemImageProperty =
            DependencyProperty.Register("ItemImage",
                                         typeof(Image),
                                         typeof(DesignerItem));
        #endregion

        #region ParentID
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(DesignerItem));
        #endregion

        #region IsGroup
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }

        #endregion

        #region ConnectorDecoratorTemplate Property

        public void SetBackgroundBrush(string brushName)
        {
            ((Path)this.Content).Fill = (Brush)this.FindResource(brushName);
        }

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        #region ItemLabel

        public string ItemLabel
        {
            get { return (string)GetValue(ItemLabelProperty); }
            set { SetValue(ItemLabelProperty, value); }
        }
        public static readonly DependencyProperty ItemLabelProperty =
            DependencyProperty.Register("ItemLabel",
                                         typeof(string),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(""));
        #endregion

        #region IsLabelInEditMode

        public bool IsLabelInEditMode
        {
            get { return (bool)GetValue(IsLabelInEditModeProperty); }
            set { SetValue(IsLabelInEditModeProperty, value); }
        }
        public static readonly DependencyProperty IsLabelInEditModeProperty =
            DependencyProperty.Register("IsLabelInEditMode",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem(Guid id)
        {
            this.id = id;            
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
            editModeTimer.Elapsed += editModeTimer_Elapsed;
        }

        void editModeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            selectedClickCounter = 0;
            editModeTimer.Stop();
        }

        public DesignerItem()
            : this(Guid.NewGuid())
        {
        }

        private DesignerCanvas designerCanvas = null;
        private System.Timers.Timer editModeTimer = new System.Timers.Timer(1500);
        private int selectedClickCounter = 0;

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            editModeTimer.Stop();
            selectedClickCounter = 0;

            RaiseEvent(new RoutedEventArgs(DesignerItem.DoubleClickEvent, this));
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            
            // update selection
            if (designerCanvas != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {                        
                        designerCanvas.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        designerCanvas.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designerCanvas.SelectionService.SelectItem(this);
                }
                Focus();
            }

            

            e.Handled = false;
        }

        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.designerCanvas == null)
            {
                designerCanvas = VisualTreeHelper.GetParent(this) as DesignerCanvas;
                designerCanvas.PreviewMouseUp += designerCanvas_PreviewMouseUp;
            }

            if (base.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    if (contentVisual != null)
                    {
                        DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                        if (thumb != null)
                        {
                            ControlTemplate template =
                                DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                            if (template != null)
                                thumb.Template = template;
                        }
                    }
                }
            }
        }

        
        void designerCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(this.IsSelected)
            {
                if(editModeTimer.Enabled == false)
                {
                    editModeTimer.Start();
                }
                selectedClickCounter++;
                if(selectedClickCounter == 2)
                {
                    this.IsLabelInEditMode = true;
                    RaiseEvent(new RoutedEventArgs(DesignerItem.ClickEvent, null));
                }
                else
                {
                    RaiseEvent(new RoutedEventArgs(DesignerItem.ClickEvent, this));
                }
            }
            else
            {
                selectedClickCounter = 0;
                this.IsLabelInEditMode = false;
            }

            
        }

    }
}
