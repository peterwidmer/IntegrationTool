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

namespace IntegrationTool.ProjectDesigner.Screens.UserControls
{
    /// <summary>
    /// Interaction logic for LogHeader.xaml
    /// </summary>
    public partial class LogHeader : UserControl
    {
        public LogHeader()
        {
            InitializeComponent();
            this.DataContext = null;
        }

        public void SetItemLog(ItemLog itemLog)
        {
            this.DataContext = itemLog;
        }

        public void SetDetailedLogVisibility(Visibility visibility)
        {
            this.lblDetailedLog.Visibility = visibility;
        }
    }
}
