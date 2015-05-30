using IntegrationTool.ApplicationCore;
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

namespace IntegrationTool.ProjectDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationInitializer applicationInitializer;
        private Project CurrentProject = null;

        public MainWindow(ApplicationInitializer applicationInitializer)
        {
            InitializeComponent();
            this.applicationInitializer = applicationInitializer;
            //this.mainWindowContent.Content = new InitializationScreens.InitialScreen();

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, New_Executed, New_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Open_Executed, Open_Enabled));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_Enabled));

            OpenProject(@"C:\Temp\IntegrationToolTests\CrmTest\CrmTest.xml");
            // OpenProject(@"..\..\..\IntegrationTool.UnitTests.ApplicationCore\FlowTestProjects\ErrorContinuation\ComplexTests.xml");
            // OpenProject(@"..\..\..\IntegrationTool.UnitTests.ApplicationCore\FlowTestProjects\ErrorPath\SimpleErrorPathTests.xml");
            
            // this.mainWindowContent.Content = new FlowDesign.FlowDesigner(applicationInitializer.ModuleLoader.Modules);


        }
    }
}
