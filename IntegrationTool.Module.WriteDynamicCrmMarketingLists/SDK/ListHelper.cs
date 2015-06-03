using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK
{
    public class ListHelper
    {
        public static Marketinglist CreateMarketingList(IOrganizationService service, string listname, MarketinglistMemberType listMemberType)
        {
            Entity list = new Entity("list");
            list.Attributes.Add("listname", listname);
            switch (listMemberType)
            {
                case MarketinglistMemberType.Account:
                    list.Attributes.Add("createdfromcode", new OptionSetValue(1));
                    list.Attributes.Add("membertype", 1);
                    break;

                case MarketinglistMemberType.Contact:
                    list.Attributes.Add("createdfromcode", new OptionSetValue(2));
                    list.Attributes.Add("membertype", 2);
                    break;

                case MarketinglistMemberType.Lead:
                    list.Attributes.Add("createdfromcode", new OptionSetValue(4));
                    list.Attributes.Add("membertype", 4);
                    break;
            }

            list.Id = service.Create(list);

            return new Marketinglist(listname, list.Id);
        }

        public static DataCollection<Entity> RetrieveMarketinglists(IOrganizationService service, string listname)
        {
            DataCollection<Entity> lists = Crm2013Wrapper.Crm2013Wrapper.RetrieveMultiple(
                service,
                "list",
                new ColumnSet(new string[] { "listid" }),
                new ConditionExpression("listname", ConditionOperator.Equal, listname));

            return lists;
        }
    }
}
