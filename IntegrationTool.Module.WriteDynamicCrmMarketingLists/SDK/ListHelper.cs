using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK
{
    public class ListHelper
    {
        public static Marketinglist CreateMarketingList(IOrganizationService service, Entity list, MarketinglistMemberType listMemberType)
        {
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

            try
            {
                list.Id = service.Create(list);
            }
            catch (Exception e)
            {
                Thread.Sleep(new TimeSpan(0, 0, 0, 5));
                list.Id = service.Create(list);
            }
            return new Marketinglist(list["listname"].ToString(), list.Id);
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
