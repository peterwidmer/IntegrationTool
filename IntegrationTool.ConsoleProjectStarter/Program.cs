using CommandLine;
using IntegrationTool.ApplicationCore;
using IntegrationTool.Flowmanagement;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ConsoleProjectStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandLineOptions commandLineOptions = ParseCommandLineOptions(args);

            ApplicationInitializer appInitializer = new ApplicationInitializer();

            Project project = ProjectLoader.LoadFromPath(commandLineOptions.ProjectPath, appInitializer);

            Type[] extraTypes = appInitializer.ModuleLoader.GetModuleTypeList();

            Console.WriteLine("Starting execution of project " + project.ProjectName + "\n");

            string[] packagesToRun = String.IsNullOrEmpty(commandLineOptions.Packages) ?
                project.Packages.Select(t => t.DisplayName).ToArray<string>() :
                packagesToRun = commandLineOptions.Packages.Split(',');

            List<Package> packagesToExecute = new List<Package>();
            foreach (string packageName in packagesToRun)
            {
                Package package = project.Packages.Where(t => t.DisplayName == packageName).FirstOrDefault();
                if(package == null)
                {
                    Console.WriteLine("Package " + packageName + " does not exist in project " + project.ProjectName);
                    Environment.Exit(1);
                }

                packagesToExecute.Add(package);
            }

            foreach(Package package in packagesToExecute)
            {
                Task.Run(() => RunPackage(package, project.Connections, appInitializer.ModuleLoader.Modules, extraTypes)).Wait();
            }

            Console.WriteLine("\nExecution finished");
        }

        static async Task RunPackage(Package package, ObservableCollection<ConnectionConfigurationBase> connections, List<ModuleDescription> modules, Type[] extraTypes)
        {
            FlowManager flowManager = new FlowManager(connections, modules, package);
            flowManager.ProgressReport += flowManager_ProgressReport;

            RunLog runLog = new RunLog();
            await flowManager.Run(runLog);
        }

        static void flowManager_ProgressReport(object sender, EventArgs e)
        {
            ProgressReport d = sender as ProgressReport;

            string message = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\t" + d.DesignerItem.ItemLabel + "\t" + d.Message;
            Console.WriteLine(message);
        }

        static CommandLineOptions ParseCommandLineOptions(string [] args)
        {
            var options = new CommandLineOptions();
            
            var parser = new Parser();
            parser.ParseArguments(args, options);

            return options;
        }

         
    }
}
