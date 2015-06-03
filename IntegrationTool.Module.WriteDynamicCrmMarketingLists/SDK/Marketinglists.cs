using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK
{
    public class Marketinglists
    {
        private Dictionary<string, Marketinglist> marketingListDictionary = new Dictionary<string, Marketinglist>();
        
        public Marketinglists()
        {
            
        }

        public Marketinglist this[string listName]
        {
            get
            {
                return marketingListDictionary[listName];
            }
            set
            {
                if (marketingListDictionary.ContainsKey(listName))
                {
                    marketingListDictionary[listName] = value;
                }
                else
                {
                    marketingListDictionary.Add(listName, value);
                }
            }
        }

        public bool Contains(string listName)
        {
            return marketingListDictionary.ContainsKey(listName);
        }

        public void Add(Marketinglist marketinglist)
        {
            this.marketingListDictionary.Add(marketinglist.ListName, marketinglist);
        }

    }
}
