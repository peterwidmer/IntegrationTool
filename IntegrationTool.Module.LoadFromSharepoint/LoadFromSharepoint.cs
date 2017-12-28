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

        public void LoadData(IConnection connection, IDatastore datastore, ReportProgressMethod reportProgress)
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
                    if (datastore.Metadata.ContainsColumn(fieldValue.Key) == false)
                    {
                        datastore.AddColumn(new ColumnMetadata(fieldValue.Key));
                    }
                }

                object[] data = new object[datastore.Metadata.Columns.Count];
                foreach (var fieldValue in listItem.FieldValues)
                {
                    if (fieldValue.Value as FieldUserValue != null)
                    {
                        data[datastore.Metadata[fieldValue.Key].ColumnIndex] = fieldValue.Value;
                    }
                    else
                    {
                        data[datastore.Metadata[fieldValue.Key].ColumnIndex] = fieldValue.Value;
                    }
                }

                datastore.AddData(data);
            }
        }
    }
}
