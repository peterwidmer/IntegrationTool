using IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.WriteDynamicCrmMarketingLists
{
    public partial class WriteToDynamicsCrmMarketingLists
    {
        private IOrganizationService service = null;
        private ReportProgressMethod reportProgress;
        private IDatastore dataObject;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            this.reportProgress = reportProgress;

            reportProgress(new SimpleProgressReport("Connection to crm"));
            CrmConnection crmConnection = (CrmConnection)connection.GetConnection();
            this.service = new OrganizationService(crmConnection);
            this.dataObject = dataObject;
            
            EntityMetadata listEntityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, "list");

            Marketinglists marketinglists = null;
            switch(this.Configuration.JoinList)
            {
                case MarketinglistJoinType.Manual:
                    marketinglists = DoManualMarketingList();
                    break;

                case MarketinglistJoinType.Join:
                    marketinglists = DoJoinMarketingLists();
                    break;
            }
        }

        public Marketinglists DoManualMarketingList()
        {
            Marketinglists marketingLists = new Marketinglists();
            reportProgress(new SimpleProgressReport("Check if manual list is already created."));

            DataCollection<Entity> lists = ListHelper.RetrieveMarketinglists(service, this.Configuration.ManualListName);

            if(lists.Count == 0)
            {
                if(this.Configuration.IfJoinUnsuccessful == OnUnsuccessfulJoin.CreateNew)
                {
                    marketingLists[this.Configuration.ManualListName] = ListHelper.CreateMarketingList(service, this.Configuration.ManualListName, this.Configuration.ListMemberType);       
                }
                else
                {
                    throw new Exception("List " + this.Configuration.ManualListName + " does not exist.");
                }
            }
            else if(lists.Count == 1)
            {
                marketingLists[this.Configuration.ManualListName] = new Marketinglist(this.Configuration.ManualListName, lists[0].Id);
            }
            else
            {
                throw new Exception("Multiple lists with the name " + this.Configuration.ManualListName + " exists.");
            }

            return marketingLists;
        }

        public Marketinglists DoJoinMarketingLists()
        {
            Marketinglists marketingLists = new Marketinglists();

            Dictionary<string, int> sourceColumnMapping = new Dictionary<string, int>();
            foreach (var mapping in this.Configuration.ListMapping)
            {
            }

            for (int i = 0; i < this.dataObject.Count; i++)
            {
                
            }

            // TODO Implement
            return marketingLists;
        }

        private string BuildKey(string [] keyvalues)
        {
            string key = string.Empty;
            for(int i=0; i < keyvalues.Length; i++)
            {
                key += keyvalues[i] + "##";
            }

            return key;
        }

    }
}
