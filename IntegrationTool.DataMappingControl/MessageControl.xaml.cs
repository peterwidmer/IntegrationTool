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

namespace IntegrationTool.DataMappingControl
{
    /// <summary>
    /// Interaction logic for MessageControl.xaml
    /// </summary>
    public partial class MessageControl : UserControl
    {
        public MessageControl(string title, string message)
        {
            InitializeComponent();
            this.lblTitle.Content = title;
            this.lblMessage.Text = message;
        }

        public void SetMessageHeight(double height)
        {
            this.rowMessageBlock.Height = new GridLength(height, GridUnitType.Star);
        }
    }
}
