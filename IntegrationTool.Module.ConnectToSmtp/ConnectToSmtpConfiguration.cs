using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToSmtp
{
    //<smtp deliveryMethod="Network" from="user@gmail.com">
    //                 <network host="smtp.gmail.com"
    //                 userName="user@gmail.com"
    //                 password="********"
    //                 port="587"
    //                 defaultCredentials="true"
    //                 enableSsl="true" />
    //         </smtp>

    public class ConnectToSmtpConfiguration : ConnectionConfigurationBase
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public SmtpDeliveryMethod DeliveryMethod { get; set; }

        public bool UseDefaultCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ConnectToSmtpConfiguration()
        {
            this.Port = 25;
            EnableSsl = false;

            UseDefaultCredentials = true;
            DeliveryMethod = SmtpDeliveryMethod.Network;
        }
    }
}
