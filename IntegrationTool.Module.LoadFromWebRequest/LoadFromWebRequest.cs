using IntegrationTool.Module.ConnectToUrl;
using IntegrationTool.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.Module.LoadFromWebRequest
{
    [SourceModuleAttribute(Name = "LoadFromWebRequest",
                           DisplayName = "Web-Request",
                           ModuleType = ModuleType.Source,
                           GroupName = ModuleGroup.Source,
                           ConnectionType = typeof(ConnectToUrlConfiguration),
                           ConfigurationType = typeof(LoadFromWebRequestConfiguration))]
    public class LoadFromWebRequest : IModule, IDataSource
    {
        public LoadFromWebRequestConfiguration Configuration { get; set; }
        public void SetConfiguration(ConfigurationBase configurationBase)
        {
            this.Configuration = configurationBase as LoadFromWebRequestConfiguration;
        }

        public System.Windows.Controls.UserControl RenderConfigurationWindow(ConfigurationBase configurationBase, SDK.Database.IDatastore dataObject)
        {
            ConfigurationWindow configurationWindow = new ConfigurationWindow((LoadFromWebRequestConfiguration)configurationBase);
            return configurationWindow;
        }

        public void LoadData(IConnection connection, SDK.Database.IDatastore datastore, ReportProgressMethod reportProgress)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp(this.Configuration.Url);
            webRequest.Method = this.Configuration.Method;
            webRequest.Accept = this.Configuration.Accept;
            webRequest.ContentType = this.Configuration.ContentType;

            var webConfiguration = connection.GetConnection() as ConnectToUrlConfiguration;
            if(webConfiguration.UseProxySettings)
            {
                // TODO Implement
            }

            if(webConfiguration.SendClientCertificate)
            {
                // TODO Implement
            }

            datastore.AddColumn(new ColumnMetadata("ResponseContent"));

            reportProgress(new SimpleProgressReport("Start retrieving data from " + this.Configuration.Url));
            using (WebResponse response = ExecuteWebRequest(webRequest))
            using (Stream dataStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(dataStream))
            {
                reportProgress(new SimpleProgressReport("Retrieved " + response.ContentLength + " bytes."));
                object [] receivedData = new object[datastore.Metadata.Columns.Count];

                string responseFromServer = reader.ReadToEnd();
                receivedData[datastore.Metadata["ResponseContent"].ColumnIndex] = responseFromServer;
            }
        }

        public WebResponse ExecuteWebRequest(HttpWebRequest webRequest)
        {
            if (!String.IsNullOrEmpty(this.Configuration.RequestContent))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(this.Configuration.RequestContent);
                webRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }

            return webRequest.GetResponse();
        }
    }
}
