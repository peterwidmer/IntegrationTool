using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ExecuteCmd
{
    [StepModuleAttribute(Name = "ExecuteCmd",
                           DisplayName = "Execute CMD",
                           ModuleType = ModuleType.Step,
                           GroupName = ModuleGroup.Task,
                           ContainsSubConfiguration = false,
                           RequiresConnection = false,
                           ConfigurationType = typeof(ExecuteCmdConfiguration))]
    public class ExecuteCmd : IModule, IStep
    {
        public ExecuteCmdConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ExecuteCmdConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ExecuteCmdConfiguration)configurationBase);
            return configurationWindow;
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = true;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + this.Configuration.CmdValue;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!String.IsNullOrEmpty(error))
                {
                    throw new Exception("An error occured on execution of " + this.Configuration.CmdValue + ":\n\n" + error);
                }
            }
        }
    }
}
