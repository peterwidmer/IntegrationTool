using IntegrationTool.DataMappingControl;
using IntegrationTool.SDK.ConfigurationsBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists
{
    public enum MarketinglistJoinType
    {
        Manual, Join
    }

    public enum OnUnsuccessfulJoin
    {
        CreateNew,
        Fail
    }

    public class WriteToDynamicsCrmMarketingListsConfiguration : TargetConfiguration
    {
        public MarketinglistJoinType JoinList { get; set; }
        public string ManualListName { get; set; }
        public OnUnsuccessfulJoin IfJoinUnsuccessful { get; set; }

        public List<DataMapping> ListMapping { get; set; }

        public List<DataMapping> ListMemberMapping { get; set; }
    }
}
