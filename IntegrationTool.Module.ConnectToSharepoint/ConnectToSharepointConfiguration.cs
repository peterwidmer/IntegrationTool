using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.ConnectToSharepoint
{
    public enum SharepointAuthenticationType
    {
        OnPremise,
        SharepointOnline
    }

    public class ConnectToSharepointConfiguration : ConnectionConfigurationBase
    {
        public string SiteUrl { get; set; }

        public SharepointAuthenticationType AuthenticationType { get; set; }
        public bool UseIntegratedSecurity { get; set; }
        public string Domain { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public ConnectToSharepointConfiguration()
        {
            UseIntegratedSecurity = true;
        }
    }
}
