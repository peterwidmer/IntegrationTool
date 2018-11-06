﻿using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromDynamicsCrm
{
    [SourceModuleAttribute(Name = "LoadFromDynamicsCrm",
                           DisplayName = "CRM",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(IOrganizationService),
                           ConfigurationType = typeof(LoadFromDynamicsCrmConfiguration))]
    public class LoadFromDynamicsCrm : IModule, IDataSource
    {
        private IOrganizationService service = null;
        private IDatastore datastore = null;
        public LoadFromDynamicsCrmConfiguration Configuration { get; set; }

        public LoadFromDynamicsCrm()
        {
            Configuration = new LoadFromDynamicsCrmConfiguration();
        }

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromDynamicsCrmConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromDynamicsCrmConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress)
        {
            this.datastore = datastore;

            if(String.IsNullOrEmpty(this.Configuration.FetchXml))
            {
                return;
            }

            reportProgress(new SimpleProgressReport("Connection to crm"));
            this.service = connection.GetConnection() as IOrganizationService;

            reportProgress(new SimpleProgressReport("Start fetching entities..."));
            Crm2013Wrapper.Crm2013Wrapper.ExecuteFetchXml(service, this.Configuration.FetchXml, FetchXmlEntityCollectionRetrieved);
        }

        private void FetchXmlEntityCollectionRetrieved(EntityCollection retrievedEntityCollection)
        {
            foreach (Entity entity in retrievedEntityCollection.Entities)
            {
                foreach (var attribute in entity.Attributes)
                {
                    if (datastore.Metadata.ContainsColumn(attribute.Key) == false)
                    {
                        datastore.AddColumn(new ColumnMetadata(attribute.Key));
                    }
                }

                object[] data = new object[datastore.Metadata.Columns.Count];

                switch (this.Configuration.QueryType)
                {
                    case DynamicsCrmQueryType.ExecuteFetchXml:
                        foreach (var attribute in entity.Attributes)
                        {
                            if (attribute.Value as AliasedValue != null)
                            {
                                data[datastore.Metadata[attribute.Key].ColumnIndex] = ((AliasedValue) attribute.Value).Value;
                            }
                            else if (attribute.Value as EntityReference != null)
                            {
                                data[datastore.Metadata[attribute.Key].ColumnIndex] = ((EntityReference) attribute.Value).Id;
                            }
                            else if (attribute.Value as OptionSetValue != null)
                            {
                                data[datastore.Metadata[attribute.Key].ColumnIndex] = ((OptionSetValue) attribute.Value).Value;
                            }
                            else if (attribute.Value as Money != null)
                            {
                                data[datastore.Metadata[attribute.Key].ColumnIndex] = ((Money) attribute.Value).Value;
                            }
                            else
                            {
                                data[datastore.Metadata[attribute.Key].ColumnIndex] = attribute.Value;
                            }
                        }
                        break;

                    case DynamicsCrmQueryType.NativeExecuteFetchXml:
                        foreach (var attribute in entity.Attributes)
                        {
                            if (attribute.Key == "ownerid")
                            {

                            }

                            {
                                data[datastore.Metadata[attribute.Key].ColumnIndex] = attribute.Value;
                            }
                        }
                        break;
                }

                datastore.AddData(data);
            }
        }
    }
}
