using IntegrationTool.ApplicationCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace IntegrationTool.ProjectDesigner
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        InitOperation _operation;

        public SplashScreen(InitOperation worker)
        {
            this._operation = worker;
            this._operation.Complete += new EventHandler(_worker_Complete);
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Splash_Loaded);
        }

        void _worker_Complete(object sender, EventArgs e)
        {
            if (_operation.Completed)
            {
                object[] result = (object[])((RunWorkerCompletedEventArgs)e).Result;
                ApplicationInitializer initializer = (ApplicationInitializer)result[0];

                MainWindow mw = new MainWindow(initializer);
                Close();
                mw.Show();
            }
            else
            {
                MessageBox.Show("Cannot initialise application:\n" + _operation.ErrorDescription,
                "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        void Splash_Loaded(object sender, RoutedEventArgs e)
        {
            _operation.Start();
        }

    }
}
