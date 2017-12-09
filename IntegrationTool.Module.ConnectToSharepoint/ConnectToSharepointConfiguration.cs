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
    }
}
