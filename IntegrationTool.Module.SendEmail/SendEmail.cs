using IntegrationTool.SDK;
using IntegrationTool.SDK.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.SendEmail
{
    [StepModuleAttribute(Name = "SendEmail",
                           DisplayName = "Send Email",
                           ModuleType = ModuleType.Step,
                           GroupName = ModuleGroup.Task,
                           ContainsSubConfiguration = false,
                           RequiresConnection = true,
                           ConnectionType = typeof(SmtpClient),
                           ConfigurationType = typeof(SendEmailConfiguration))]
    public class SendEmail : IModule, IStep
    {
        public SendEmailConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as SendEmailConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((SendEmailConfiguration)configurationBase);
            return configurationWindow;
        }

        public void Execute(IConnection connection, SDK.Database.IDatabaseInterface databaseInterface, ReportProgressMethod reportProgress)
        {
            SmtpClient smtpClient = connection.GetConnection() as SmtpClient;
            MailMessage mailMessage = new MailMessage(
                this.Configuration.From, 
                this.Configuration.To, 
                this.Configuration.Subject,
                this.Configuration.Body);
            mailMessage.IsBodyHtml = this.Configuration.IsBodyHtml;
            if (String.IsNullOrEmpty(this.Configuration.Cc) == false)
            {
                mailMessage.CC.Add(new MailAddress(this.Configuration.Cc));
            }

            smtpClient.Send(mailMessage);
        }
    }
}
