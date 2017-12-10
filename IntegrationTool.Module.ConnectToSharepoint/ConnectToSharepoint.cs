using IntegrationTool.SDK;
using IntegrationTool.SDK.Module.ModuleAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTool.SDK.Database;
using System.Windows.Controls;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Net;

namespace IntegrationTool.Module.ConnectToSharepoint
{
    [ConnectionModuleAttribute(
        DisplayName = "Sharepoint",
        Name = "ConnectToSharepoint",
        ContainsSubConfiguration = false,
        ModuleType = ModuleType.Connection,
        ConnectionType = typeof(ClientContext),
        ConfigurationType = typeof(ConnectToSharepointConfiguration))]
    public class ConnectToSharepoint : IModule, IConnection
    {
        public ConnectToSharepointConfiguration Configuration { get; set; }

        public UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((ConnectToSharepointConfiguration)configurationBase);
            return configurationWindow;
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as ConnectToSharepointConfiguration;
        }

        public object GetConnection()
        {
            var clientContext = new ClientContext(this.Configuration.SiteUrl);
            if(Configuration.AuthenticationType == SharepointAuthenticationType.OnPremise)
            {
                if(Configuration.UseIntegratedSecurity)
                {
                    clientContext.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                else
                {
                    clientContext.Credentials = new NetworkCredential(Configuration.User, Configuration.Password, Configuration.Domain);
                }
            }
            else if(Configuration.AuthenticationType == SharepointAuthenticationType.SharepointOnline)
            {
                var securePassword = new SecureString();
                foreach (char c in Configuration.Password)
                {
                    securePassword.AppendChar(c);
                }

                clientContext.Credentials = new SharePointOnlineCredentials(Configuration.User, securePassword);
            }
            return clientContext;
        }
    }
}
