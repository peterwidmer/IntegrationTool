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

namespace IntegrationTool.SDK.Controls.Generic
{
    /// <summary>
    /// Interaction logic for LogSummaryControl.xaml
    /// </summary>
    public partial class LogSummaryControl : UserControl
    {
        public LogSummaryControl()
        {
            InitializeComponent();
        }

        public void SetModel(LogSummary logSummary)
        {
            this.DataContext = logSummary;
        }
    }
}
