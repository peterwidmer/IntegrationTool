using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK
{
    public class Marketinglist
    {
        public string ListName { get; set; }
        public Guid ListId { get; set; }

        public Marketinglist() {}

        public Marketinglist(string listName)
        {
            this.ListName = listName;
        }

        public Marketinglist(string listName, Guid listId)
        {
            this.ListName = listName;
            this.ListId = listId;
        }
    }
}
