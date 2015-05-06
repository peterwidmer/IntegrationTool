using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using DataMappingControl;


namespace IntegrationTool.DataMappingControl
{
    public delegate void MappedRowHandler(DataMapping item);

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MappingControl : INotifyPropertyChanged
    {
        private const int ITEMHEIGHT = 18;
        public event MappedRowHandler MappingRowAdded;
        public event MappedRowHandler MappingRowDeleted;

        public bool StoreTooltipInTargetMappingInsteadOfContent;
        public bool StoreTooltipInSourceMappingInsteadOfContent;

        private ObservableCollection<ListViewItem> sourceList;
        public ObservableCollection<ListViewItem> SourceList
        {
            get { return this.sourceList; }
            set
            {
                sourceList = value;
                this.OnPropertyChanged("SourceList");
            }
        }

        private ObservableCollection<ListViewItem> targetList;
        public ObservableCollection<ListViewItem> TargetList
        {
            get { return this.targetList; }
            set
            {
                targetList = value;
                this.OnPropertyChanged("TargetList");
            }
        }

        private List<DataMapping> mapping;
        public List<DataMapping> Mapping
        {
            get { return this.mapping; }
            set
            {
                mapping = value;
                this.OnPropertyChanged("Mapping");
            }
        }

        DragHelper dragHelper;
        DropHelper dropHelper;
        Double verticaloffset;
        Double verticaoffsets;

        public MappingControl()
        {
            this.InitializeComponent();
            this.Mapping = new List<DataMapping>();

            this.TargetList = new ObservableCollection<ListViewItem>();
            this.SourceList = new ObservableCollection<ListViewItem>();

            InitializeComponent();
            this.mainCanvas.AllowDrop = true;

            ListBoxDragDropDataProvider callback = new ListBoxDragDropDataProvider(this.ListBoxSource);
            dragHelper = new DragHelper(this.ListBoxSource, callback, this.LayoutRoot);
            dropHelper = new DropHelper(this.ListBoxTarget, this);

            ListBoxTarget.SelectedIndex = -1;

            ListBoxSource.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
            ListBoxTarget.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
        }

        private void myScrollVievert_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            this.verticaloffset = myScrollVievert.VerticalOffset;
            this.RedrawLines();
        }

        private void myScrollViever_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.verticaoffsets = myScrollViever.VerticalOffset;
            this.RedrawLines();
            myScrollViever2.ScrollToVerticalOffset(myScrollViever.VerticalOffset);
        }

        public void DrawLine(ListViewItem sourceItem, ListViewItem targetItem, bool isnew)
        {
            try
            {
                if (sourceItem != null)
                {
                    DrawingGroup hu = VisualTreeHelper.GetDrawing(sourceItem);
                    Line drawLine = new Line();
                    int position = (ListBoxSource.Items.IndexOf(sourceItem));

                    drawLine.X1 = ListBoxSource.ActualWidth; //X of LineStart
                    drawLine.Y1 = 9 + (position * ITEMHEIGHT);       //Y of LineStart
                    double test = ListBoxTarget.Margin.Left;

                    drawLine.X2 = mainCanvas.ActualWidth; //X of LineEnd
                    drawLine.Y2 = 9;                      //Y of LineEnd
                    drawLine.Y2 -= (verticaloffset * 1);  //1.0021
                    drawLine.Y2 += (verticaoffsets * 1);
                    int post = (ListBoxTarget.Items.IndexOf(targetItem));
                    drawLine.Y2 += (post * ITEMHEIGHT);
                    drawLine.Stroke = new SolidColorBrush(Color.FromRgb(58, 162, 230));

                    sourceItem.Height = ITEMHEIGHT;
                    sourceItem.Background = new SolidColorBrush(Color.FromRgb(58, 162, 230));
                    sourceItem.Foreground = Brushes.White;

                    drawLine.StrokeThickness = 1;

                    String name = GetLineName(sourceItem.Content.ToString(), targetItem.Content.ToString());

                    drawLine.Name = name;

                    if (!isnew)
                        mainCanvas.Children.Add(drawLine);

                    drawLine.MouseDown += new MouseButtonEventHandler(myLine_MouseDown);

                    if (isnew)
                    {
                        Boolean isallredimapped = false;
                        foreach (DataMapping item in mapping)
                        {
                            if (GetLineName(item.Source, item.Target) == name)
                                isallredimapped = true;
                        }
                        if (!isallredimapped)
                        {
                            mainCanvas.Children.Add(drawLine);
                            DataMapping r = new DataMapping()
                            {
                                Target = StoreTooltipInTargetMappingInsteadOfContent == false ? targetItem.Content.ToString() : targetItem.ToolTip.ToString(),
                                Source = StoreTooltipInSourceMappingInsteadOfContent == false ? sourceItem.Content.ToString() : sourceItem.ToolTip.ToString()
                            };
                            Mapping.Add(r);

                            if (this.MappingRowAdded != null)
                                MappingRowAdded(r);

                        }
                    }
                }
            }
            catch (Exception e)
            {

                MessageBox.Show(e.ToString());
            }
        }

        public void RedrawLines()
        {
            mainCanvast.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            mainCanvast.Height = (targetList.Count * ITEMHEIGHT) + 40;
            if (this.ActualHeight > (sourceList.Count * ITEMHEIGHT) + 40)
                mainCanvas.Height = this.ActualHeight;
            else
                mainCanvas.Height = sourceList.Count * ITEMHEIGHT + 40;

            ccMainCanvas.Height = (sourceList.Count * ITEMHEIGHT) + 40;

            foreach (ListViewItem item in sourceList)
            {
                item.Height = ITEMHEIGHT;
                item.FontSize = 11;
                item.Background = Brushes.White;
            }

            foreach (ListViewItem item in targetList)
            {
                item.Height = ITEMHEIGHT;
                item.FontSize = 11;
                item.Background = Brushes.White;
            }

            List<Line> delList = mainCanvas.Children.OfType<Line>().ToList();
            List<string> delLineList = new List<string>();

            for (int i = 0; i < delList.Count; i++)
            {
                mainCanvas.Children.Remove(delList[i]);
            }

            foreach (DataMapping mapping in Mapping)
            {
                ListViewItem sourceitem = ListBoxSource.Items.OfType<ListViewItem>().
                                            Where(t =>
                                                StoreTooltipInSourceMappingInsteadOfContent == false ? 
                                                        t.Content.ToString() == mapping.Source :
                                                        t.ToolTip.ToString() == mapping.Source).FirstOrDefault();
                ListViewItem targetitem = ListBoxTarget.Items.OfType<ListViewItem>().
                                            Where(t =>
                                                StoreTooltipInTargetMappingInsteadOfContent == false ?
                                                        t.Content.ToString() == mapping.Target :
                                                        t.ToolTip.ToString() == mapping.Target).FirstOrDefault();

                if (sourceitem != null && targetitem != null)
                    this.DrawLine(sourceitem, targetitem, false);
                else
                    delLineList.Add(GetLineName(mapping.Source, mapping.Target));

            }

            foreach (string toldelkey in delLineList)
            {
                var row = (from r in mapping.AsEnumerable() where GetLineName(r.Source, r.Target) == toldelkey select r).First();
                mapping.Remove(row);
            }


        }

        void myLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Delete Relationship?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Line line = (Line)sender;
                mainCanvas.Children.Remove(line);

                var query =
                from entrys in Mapping
                where GetLineName(entrys.Source, entrys.Target) == line.Name
                select entrys;

                var todelete = query.FirstOrDefault();

                if (todelete != null)
                {
                    ListViewItem sourceItem = null;

                    foreach (ListViewItem sitem in ListBoxSource.Items)
                    {
                        if (sitem.Content.ToString() == todelete.Source)
                            sourceItem = sitem;
                    }

                    sourceItem.Height = ITEMHEIGHT;
                    sourceItem.Background = Brushes.White;

                    DataMapping r = new DataMapping()
                    {
                        Source = todelete.Source,
                        Target = todelete.Target
                    };

                    Mapping.Remove(todelete);

                    if (this.MappingRowDeleted != null)
                        MappingRowDeleted(r);
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SortLeftSide(object sender, RoutedEventArgs e)
        {
            ListBoxSource.Items.SortDescriptions.Clear();
            if (btnSortDownSl.Tag.ToString() == "asc")
            {
                ListBoxSource.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
                btnSortDownSl.Content = "▲";
                btnSortDownSl.Tag = "desc";
            }
            else
            {
                ListBoxSource.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Descending));
                btnSortDownSl.Content = "▼";
                btnSortDownSl.Tag = "asc";
            }
            RedrawLines();
        }

        private void SortRightSide(object sender, RoutedEventArgs e)
        {
            ListBoxTarget.Items.SortDescriptions.Clear();
            if (btnSortDownTl.Tag.ToString() == "asc")
            {
                ListBoxTarget.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Ascending));
                btnSortDownTl.Content = "▲";
                btnSortDownTl.Tag = "desc";
            }
            else
            {
                ListBoxTarget.Items.SortDescriptions.Add(new SortDescription("Content", ListSortDirection.Descending));
                btnSortDownTl.Content = "▼";
                btnSortDownTl.Tag = "asc";
            }
            RedrawLines();
        }


        private void btnSortDel_Click(object sender, RoutedEventArgs e)
        {
            ListBoxSource.Items.SortDescriptions.Clear();
            RedrawLines();
        }

        private void btnSortDel2_Click(object sender, RoutedEventArgs e)
        {
            ListBoxTarget.Items.SortDescriptions.Clear();
            RedrawLines();
        }

        public event EventHandler ListBoxTargetKlick;

        private void ListBoxSource_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxSourceKlick != null)
                ListBoxSourceKlick.Invoke(sender, e);
        }

        public event EventHandler ListBoxSourceKlick;


        public void UpdateMappingtable()
        {
            List<DataMapping> tempTable = new List<DataMapping>();
            foreach (DataMapping dm in mapping)
            {
                tempTable.Add(new DataMapping()
                    {
                        Source = dm.Source,
                        Target = dm.Target
                    });
            }
            mapping.Clear();

            foreach (DataMapping item in tempTable)
            {
                ListViewItem sourceItem = null;
                foreach (ListViewItem sitem in ListBoxSource.Items)
                {
                    if (sitem.Content.ToString() == item.Source)
                        sourceItem = sitem;
                }

                ListViewItem targetItem = null;


                foreach (ListViewItem titem in ListBoxTarget.Items)
                {
                    if (titem.Content.ToString() == item.Target)
                        targetItem = titem;
                }

                if (sourceItem != null && targetItem != null)
                {
                    DataMapping r = new DataMapping()
                    {
                        Source = item.Source,
                        Target = item.Target
                    };

                    mapping.Add(r);
                }
            }
        }

        public string GetSourceValueByTargetValue(string TargetValue)
        {
            foreach (DataMapping drMapping in mapping)
            {
                if (drMapping.Target.Equals(TargetValue))
                    return drMapping.Source;
            }

            return string.Empty;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListBoxSource.Width = btnSortDownSl.ActualWidth;
        }


        private static string GetLineName(string Source, string Target)
        {            
            string lineName = "Line" + Source + Target;
            Regex objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
            return objAlphaNumericPattern.Replace(lineName, "_");
        }

        private void ListBoxTarget_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxTarget.SelectedItem != null && ListBoxTargetKlick != null)
            {
                ListBoxTargetKlick(ListBoxTarget.SelectedItem, e);
            }
        }
    }
}
