using IntegrationTool.ProjectDesigner.Classes;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
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

namespace IntegrationTool.ProjectDesigner.MenuWindows
{
    /// <summary>
    /// Interaction logic for NewPackage.xaml
    /// </summary>
    public partial class NewPackage : Window
    {
        public WindowResult Status { get; set; }
        public Package Package { get; set; }
        
        public NewPackage()
        {
            InitializeComponent();
            this.DataContext = this.Package = new Package();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            this.Status = WindowResult.Created;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Status = WindowResult.Canceled;
            this.Close();
        }
    }
}
