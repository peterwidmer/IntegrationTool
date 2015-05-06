using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DataMappingControl
{
    public class DragDataWrapper
    {
        public DependencyObject Source;
        public object Data;
        public bool AllowChildrenRemove;
        public IDataDropObjectProvider Shim;
    }
}
