using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ConsoleProjectStarter
{
    public class CommandLineOptions
    {
      [Option('p', "projectpath", Required = true, HelpText = "Full path to the project file. If no package is passed, all packages will be executed!")]
      public string ProjectPath { get; set; }

      [Option('n', "packagenames", DefaultValue="", HelpText = "Comma seperated list of package(s)")]
      public string Packages { get; set; }

      [HelpOption]
      public string GetUsage()
      {
        // this without using CommandLine.Text
        var usage = new StringBuilder();
        usage.AppendLine("IntegrationTool ConsoleProjectStarter 1.0");
        usage.AppendLine("Read user manual for usage instructions...");
        return usage.ToString();
      }
    }
}
