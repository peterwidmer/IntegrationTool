using IntegrationTool.Module.CrmWrapper;
using IntegrationTool.Module.WriteDynamicCrmMarketingLists.SDK;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Crm.Sdk.Messages;
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
        private EntityMetadata listEntityMetaData;
        private ReportProgressMethod reportProgress;
        private IDatastore dataObject;

        private Dictionary<string, Guid[]> existingLists;
        private Dictionary<string, Guid[]> existingMembers;

        public void WriteData(IConnection connection, IDatabaseInterface databaseInterface, IDatastore dataObject, ReportProgressMethod reportProgress)
        {
            this.reportProgress = reportProgress;

            reportProgress(new SimpleProgressReport("Connection to crm"));
            this.service = connection.GetConnection() as IOrganizationService;
            this.dataObject = dataObject;

            reportProgress(new SimpleProgressReport("Load marketinglist metadata"));
            this.listEntityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, "list");

            reportProgress(new SimpleProgressReport("Resolve existing marketinglists"));
            JoinResolver listResolver = new JoinResolver(this.service, listEntityMetaData, this.Configuration.ListMapping);
            this.existingLists = listResolver.BuildMassResolverIndex();

            reportProgress(new SimpleProgressReport("Load members metadata"));
            EntityMetadata memberEntityMetaData = Crm2013Wrapper.Crm2013Wrapper.GetEntityMetadata(service, this.Configuration.ListMemberType.ToString().ToLower());
            
            reportProgress(new SimpleProgressReport("Resolve listmembers"));
            JoinResolver memberResolver = new JoinResolver(this.service, memberEntityMetaData, this.Configuration.ListMemberMapping);
            this.existingMembers = memberResolver.BuildMassResolverIndex();

            switch(this.Configuration.JoinList)
            {
                case MarketinglistJoinType.Manual:
                    DoManualMarketingList();
                    break;

                case MarketinglistJoinType.Join:
                    DoJoinMarketingLists();
                    break;
            }
        }

        public void AddMemberToList(Guid listId, object [] currentRecord)
        {
            string joinKey = JoinResolver.BuildExistingCheckKey(currentRecord, this.Configuration.ListMemberMapping, this.dataObject.Metadata);
            foreach(Guid memberId in this.existingMembers[joinKey])
            {
                AddMemberListRequest addMemberListRequest = new AddMemberListRequest();
                addMemberListRequest.ListId = listId;
                addMemberListRequest.EntityId = memberId;

                this.service.Execute(addMemberListRequest);
            }
        }

        public void DoManualMarketingList()
        {
            Marketinglist marketinglist = null;

            reportProgress(new SimpleProgressReport("Check if manual list is already created."));

            DataCollection<Entity> lists = ListHelper.RetrieveMarketinglists(service, this.Configuration.ManualListName);

            if(lists.Count == 0)
            {
                if(this.Configuration.IfJoinUnsuccessful == OnUnsuccessfulJoin.CreateNew)
                {
                    Entity list = new Entity("list");
                    list.Attributes.Add("listname", this.Configuration.ManualListName);
                    marketinglist = ListHelper.CreateMarketingList(service, list, this.Configuration.ListMemberType);       
                }
                else
                {
                    throw new Exception("List " + this.Configuration.ManualListName + " does not exist.");
                }
            }
            else if(lists.Count == 1)
            {
                marketinglist = new Marketinglist(this.Configuration.ManualListName, lists[0].Id);
            }
            else
            {
                throw new Exception("Multiple lists with the name " + this.Configuration.ManualListName + " exists.");
            }

            for (int i = 0; i < this.dataObject.Count; i++)
            {
                AddMemberToList(marketinglist.ListId, this.dataObject[i]);
            }
        }

        public void DoJoinMarketingLists()
        {
            EntityMapperLight entityMapper = new EntityMapperLight(this.listEntityMetaData, this.dataObject.Metadata, this.Configuration.ListMapping);

            for (int i = 0; i < this.dataObject.Count; i++)
            {
                string joinKey = JoinResolver.BuildExistingCheckKey(this.dataObject[i], this.Configuration.ListMapping, this.dataObject.Metadata);
                if (existingLists.ContainsKey(joinKey))
                {
                    if (existingLists[joinKey].Length > 1)
                    {
                        throw new Exception("Multiple lists with the joinvalues " + joinKey.Replace("#", " ").TrimEnd(' ') + " exists.");
                    }
                }
                else
                {
                    if (this.Configuration.IfJoinUnsuccessful == OnUnsuccessfulJoin.CreateNew)
                    {
                        Entity list = new Entity("list");
                        entityMapper.MapAttributes(list, this.dataObject[i]);
                        Marketinglist marketingList = ListHelper.CreateMarketingList(service, list, this.Configuration.ListMemberType);
                        existingLists.Add(joinKey, new Guid[] { marketingList.ListId });
                    }
                    else
                    {
                        throw new Exception("List with joinvalues " + joinKey.Replace("#", " ").TrimEnd(' ') + " does not exist.");
                    }
                }

                AddMemberToList(existingLists[joinKey][0], this.dataObject[i]);
            }
        }

        

    }
}
