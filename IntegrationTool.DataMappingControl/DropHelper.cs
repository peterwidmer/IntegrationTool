using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using IntegrationTool.DataMappingControl;

namespace DataMappingControl
{
    public class DropHelper
    {
        UIElement _dropTarget = null;
        MappingControl mw = null;

        string[] _datatypes = { typeof(UIElement).ToString(), "Text" };

        public string[] AllowedDataTypes
        {
            get { return _datatypes; }
            set
            {
                _datatypes = value;
                for (int x = 0; x < _datatypes.Length; x++)
                {
                    _datatypes[x] = _datatypes[x].ToLower();
                }
            }
        }

        public DropHelper(UIElement wrapper, MappingControl mw)
        {
            this.mw = mw;
            System.Diagnostics.Debug.Assert(wrapper != null);
            _dropTarget = wrapper;

            _dropTarget.AllowDrop = true;
            _dropTarget.DragEnter += new DragEventHandler(_dropTarget_DragEnter);
            _dropTarget.DragOver += new DragEventHandler(DropTarget_DragOver);
            _dropTarget.Drop += new DragEventHandler(DropTarget_Drop);
            _dropTarget.DragLeave += new DragEventHandler(_dropTarget_DragLeave);

        }


        void DropTarget_Drop(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            DragDataWrapper dw = data.GetData(typeof(DragDataWrapper).ToString()) as DragDataWrapper;


            UIElement h = (UIElement)e.OriginalSource;
            UIElement t1 = (UIElement)VisualTreeHelper.GetParent(h);
            UIElement t2 = (UIElement)VisualTreeHelper.GetParent(t1);
            UIElement t3 = (UIElement)VisualTreeHelper.GetParent(t2);

            if (t3 is ListViewItem)
            {
                mw.DrawLine((ListViewItem)dw.Data, (ListViewItem)t3, true);
            }
            else
            {
                if (t1 is ListViewItem)
                {
                    mw.DrawLine((ListViewItem)dw.Data, (ListViewItem)t1, true);
                }

            }
        }


        private DragDropEffects _allowedEffects;

        public DragDropEffects AllowedEffects
        {
            get { return _allowedEffects; }
            set { _allowedEffects = value; }
        }

        ListViewItem citem;

        void DropTarget_DragOver(object sender, DragEventArgs e)
        {
            UIElement h = (UIElement)e.OriginalSource;
            UIElement t1 = (UIElement)VisualTreeHelper.GetParent(h);
            UIElement t2 = (UIElement)VisualTreeHelper.GetParent(t1);
            UIElement t3 = (UIElement)VisualTreeHelper.GetParent(t2);

            if (t3 is ListViewItem)
            {
                ListViewItem t = ((ListViewItem)t3);
                if (t != citem)
                {
                    if (citem != null)
                    {
                        citem.IsSelected = false;
                    }
                    citem = t;
                    t.IsSelected = true;
                }
            }
        }

        private Brush _oldBackground;
        private Brush _oldBorderBrush;
        private Thickness _oldBorderThickness;

        void Highlight(object sender, DragEventArgs e)
        {

            if (_dropTarget is Panel)
            {
                Panel panel = _dropTarget as Panel;
                this._oldBackground = panel.Background;
                panel.Background = Brushes.LawnGreen;

            }
            else if (_dropTarget is Control)
            {
                Control c = _dropTarget as Control;
                this._oldBorderThickness = c.BorderThickness;
                this._oldBorderBrush = c.BorderBrush;
                c.BorderBrush = Brushes.Gray;
                c.BorderThickness = new Thickness(1);
            }
        }

        void DeHighlight(object sender, DragEventArgs e)
        {

            if (_dropTarget is Panel)
            {
                Panel panel = _dropTarget as Panel;
                panel.Background = this._oldBackground;


            }
            else if (_dropTarget is Control)
            {
                Control c = _dropTarget as Control;
                c.BorderThickness = this._oldBorderThickness;
                c.BorderBrush = this._oldBorderBrush;
            }
        }

        void _dropTarget_DragLeave(object sender, DragEventArgs e)
        {
            //  DeHighlight(sender, e);
        }

        void _dropTarget_DragEnter(object sender, DragEventArgs e)
        {
            //  Highlight(sender, e);
        }

        void selectItemunderMouse(ListView list)
        {
            int index = -1;
            for (int i = 0; i < list.Items.Count; ++i)
            {
                try
                {
                    object h = list.Items[i];
                    ListViewItem item = (ListViewItem)h;

                    string ttth = item.ActualHeight.ToString();

                    if (this.IsMouseOver(item))
                    {
                        index = i;
                        break;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
            list.SelectedItem = index;
        }

        bool IsMouseOver(Visual target)
        {
            // We need to use MouseUtilities to figure out the cursor
            // coordinates because, during a drag-drop operation, the WPF
            // mechanisms for getting the coordinates behave strangely.

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = MouseUtilities.GetMousePosition(target);
            return bounds.Contains(mousePos);
        }


    }




    public class MouseUtilities
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Win32Point pt);

        /// <summary>
        /// Returns the mouse cursor location.  This method is necessary during 
        /// a drag-drop operation because the WPF mechanisms for retrieving the
        /// cursor coordinates are unreliable.
        /// </summary>
        /// <param name="relativeTo">The Visual to which the mouse coordinates will be relative.</param>
        public static Point GetMousePosition(Visual relativeTo)
        {
            Win32Point mouse = new Win32Point();
            GetCursorPos(ref mouse);

            // Using PointFromScreen instead of Dan Crevier's code (commented out below)
            // is a bug fix created by William J. Roberts.  Read his comments about the fix
            // here: http://www.codeproject.com/useritems/ListViewDragDropManager.asp?msg=1911611#xx1911611xx
            return relativeTo.PointFromScreen(new Point((double)mouse.X, (double)mouse.Y));

            #region Commented Out
            //System.Windows.Interop.HwndSource presentationSource =
            //    (System.Windows.Interop.HwndSource)PresentationSource.FromVisual( relativeTo );
            //ScreenToClient( presentationSource.Handle, ref mouse );
            //GeneralTransform transform = relativeTo.TransformToAncestor( presentationSource.RootVisual );
            //Point offset = transform.Transform( new Point( 0, 0 ) );
            //return new Point( mouse.X - offset.X, mouse.Y - offset.Y );
            #endregion // Commented Out
        }
    }
}
