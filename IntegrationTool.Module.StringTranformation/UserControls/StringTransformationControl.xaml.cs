using IntegrationTool.Module.StringTranformation.SDK.Enums;
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

namespace IntegrationTool.Module.StringTranformation.UserControls
{
    /// <summary>
    /// Interaction logic for StringTransformationControl.xaml
    /// </summary>
    public partial class StringTransformationControl : UserControl
    {
        public StringTransformationControl()
        {
            InitializeComponent();
        }

        private void ddStringTransformationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = ddStringTransformationType.SelectedItem as ComboBoxItem;
            StringTransformationType transformationType =  (StringTransformationType)Enum.Parse(typeof(StringTransformationType), selectedItem.Tag.ToString());

            switch(transformationType)
            {
                case StringTransformationType.Replace:
                    SetParam1(System.Windows.Visibility.Visible, "Replace");
                    SetParam2(System.Windows.Visibility.Visible, "With");
                    break;

                case StringTransformationType.TrimStart:
                    SetParam1(System.Windows.Visibility.Visible, "Trim character");
                    SetParam2(System.Windows.Visibility.Hidden, null);
                    break;

                case StringTransformationType.TrimEnd:
                    SetParam1(System.Windows.Visibility.Visible, "Trim character");
                    SetParam2(System.Windows.Visibility.Hidden, null);
                    break;
            }
        }

        private void SetParam1(System.Windows.Visibility visibility, string label)
        {
            lblParam1.Visibility = visibility;
            lblParam1.Content = label;
            tbParam1.Visibility = visibility;
        }

        private void SetParam2(System.Windows.Visibility visibility, string label)
        {
            lblParam2.Visibility = visibility;
            lblParam2.Content = label;
            tbParam2.Visibility = visibility;
        }
    }
}
