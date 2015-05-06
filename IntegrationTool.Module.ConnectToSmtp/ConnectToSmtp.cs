using IntegrationTool.SDK;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToSmtp
{
    [ConnectionModuleAttribute(
        DisplayName = "SMTP",
        Name = "ConnectToSmtp",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(SmtpClient),
        ConfigurationType = typeof(ConnectToSmtpConfiguration))]
    public class ConnectToSmtp : IModule, IConnection
    {
        public ConnectToSmtpConfiguration Configuration { get; set; }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToSmtpConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToSmtpConfiguration)configurationBase);
            return configurationWindow;
        }

        public object GetConnection()
        {
            SmtpClient smtpClient = new SmtpClient(this.Configuration.Host, this.Configuration.Port);
            smtpClient.EnableSsl = this.Configuration.EnableSsl;
            smtpClient.DeliveryMethod = this.Configuration.DeliveryMethod;

            if(this.Configuration.UseDefaultCredentials == false)
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(this.Configuration.UserName, this.Configuration.Password);
            }

            return smtpClient;
        }
    }
}
