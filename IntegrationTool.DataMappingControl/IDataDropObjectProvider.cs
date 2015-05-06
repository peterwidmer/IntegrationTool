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

namespace DataMappingControl
{
    public interface IDataDropObjectProvider
    {
        //Flag of actions sypported by implementation of  IDataDropObjectProvider
        DragDropProviderActions SupportedActions { get; }

        // Called before StartDrag () to get the Data () to be used in the DataObject 
        object GetData();

        // Called before StartDrag () to add other formats , this way you can drag drop externally.. 
        void AppendData(ref IDataObject data, MouseEventArgs e);
        // Called to get the visual ( UIElement visual brush of the object being dragged..         
        UIElement GetVisual(MouseEventArgs e);
        // Gives feedback during Drag 
        void GiveFeedback(GiveFeedbackEventArgs args);
        // implements ContinueDrag -- to canceld the D&D.. 
        void ContinueDrag(QueryContinueDragEventArgs args);
        // called by the TARGET object .. this will attempt to "unparent" the current child so we can add it a child some where else.. 
        bool UnParent();

    }
}
