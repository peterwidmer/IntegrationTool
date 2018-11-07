using IntegrationTool.SDK;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTool.SDK.Database;

namespace IntegrationTool.Module.LoadFromSharepoint
{
    [SourceModuleAttribute(Name = "LoadFromSharepoint",
                           DisplayName = "Sharepoint",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(ClientContext),
                           ConfigurationType = typeof(LoadFromSharepointConfiguration))]
    public class LoadFromSharepoint : IModule, IDataSource
    {
        public LoadFromSharepointConfiguration Configuration { get; set; }
        

        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromSharepointConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, IDatastore dataObject)
        {
            return new ConfigurationWindow((LoadFromSharepointConfiguration)configurationBase);
        }

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress, bool mappingPreview)
        {
            var clientContext = connection.GetConnection() as ClientContext;

            var oList = clientContext.Web.Lists.GetByTitle(this.Configuration.ListName);
            var camlQuery = new CamlQuery() { ViewXml = this.Configuration.CamlQueryViewXml };
            var collListItem = oList.GetItems(camlQuery);

            clientContext.Load(collListItem);
            clientContext.ExecuteQuery();

            foreach (ListItem listItem in collListItem)
            {
                foreach (var fieldValue in listItem.FieldValues)
                {
                    ExtractTableHeaders(datastore, fieldValue);
                }

                object[] data = new object[datastore.Metadata.Columns.Count];
                foreach (var fieldValue in listItem.FieldValues)
                {
                    if (fieldValue.Value as FieldUserValue != null)
                    {
                        var fieldUserValue = (FieldUserValue)fieldValue.Value;
                        data[datastore.Metadata[fieldValue.Key + "_LookupId"].ColumnIndex] = fieldUserValue.LookupId;
                        data[datastore.Metadata[fieldValue.Key + "_LookupValue"].ColumnIndex] = fieldUserValue.LookupValue;
                    }
                    else if (fieldValue.Value as FieldLookupValue != null)
                    {
                        var fieldLookupValue = (FieldLookupValue)fieldValue.Value;
                        data[datastore.Metadata[fieldValue.Key + "_LookupId"].ColumnIndex] = fieldLookupValue.LookupId;
                        data[datastore.Metadata[fieldValue.Key + "_LookupValue"].ColumnIndex] = fieldLookupValue.LookupValue;
                    }
                    else
                    {
                        data[datastore.Metadata[fieldValue.Key].ColumnIndex] = fieldValue.Value;
                    }
                }

                datastore.AddData(data);
            }
        }

        private void ExtractTableHeaders(IDatastore datastore, KeyValuePair<string,object> fieldValue)
        {
            if (datastore.Metadata.ContainsColumn(fieldValue.Key) == false)
            {
                if (fieldValue.Value as FieldUserValue != null || fieldValue.Value as FieldLookupValue != null)
                {
                    datastore.AddColumn(new ColumnMetadata(fieldValue.Key + "_LookupId"));
                    datastore.AddColumn(new ColumnMetadata(fieldValue.Key + "_LookupValue"));
                }
                else
                {
                    datastore.AddColumn(new ColumnMetadata(fieldValue.Key));
                }
            }
        }
    }
}
