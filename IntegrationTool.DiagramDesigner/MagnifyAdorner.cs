using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IntegrationTool.DiagramDesigner
{
    public class MagnifyAdorner : Adorner
    {
        private DesignerCanvas designerCanvas;
        private Connection connection;

        private System.Timers.Timer doubleClickTimer = new System.Timers.Timer(400);

        public MagnifyAdorner(DesignerCanvas designer, Connection connection)
            : base(designer)
        {
            this.designerCanvas = designer;
            this.connection = connection;
            doubleClickTimer.Elapsed += doubleClickTimer_Elapsed;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            string [] resources = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            ImageSource lockImage = new BitmapImage(new Uri("pack://application:,,,/IntegrationTool.DiagramDesigner;component/resources/images/Magnify.png"));
            double x = connection.FirstElementCenterPosition.X;// +20;
            double y = connection.FirstElementCenterPosition.Y; // +20;
            Rect targetRect = new Rect(x, y, 16, 16);
            drawingContext.DrawImage(lockImage, targetRect);
        }

        int doubleClickCounter = 0;
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            doubleClickTimer.Start();
            doubleClickCounter++;

            if(doubleClickCounter == 2)
            {
                ResetDoubeCLick();
                designerCanvas.RaiseMagnifyDoubleClick(this.connection.Source.ParentDesignerItem);
            }
        }

        void doubleClickTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResetDoubeCLick();
        }

        private void ResetDoubeCLick()
        {
            doubleClickTimer.Stop();
            doubleClickCounter = 0;
        }

    }
}
