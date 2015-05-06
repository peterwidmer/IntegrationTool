using System;
using System.Collections.Generic;
using System.Data;
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

namespace UserControlTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dm.SourceList.Add(new ListViewItem() { Content = "1" });
            dm.SourceList.Add(new ListViewItem() { Content = "2" });
            dm.TargetList.Add(new ListViewItem() { Content = "3" });
            dm.TargetList.Add(new ListViewItem() { Content = "4" });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dt = dm.Mapping;
        }
    }
}
