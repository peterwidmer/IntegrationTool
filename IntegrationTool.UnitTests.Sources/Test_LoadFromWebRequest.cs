using IntegrationTool.Module.ConnectToUrl;
using IntegrationTool.Module.LoadFromWebRequest;
using IntegrationTool.SDK;
using IntegrationTool.SDK.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.Sources
{
    [TestClass]
    public class Test_LoadFromWebRequest
    {
        [TestMethod]
        public void Webrequest_NoProxy_NoCertificate_Method_Get()
        {
            ConnectToUrlConfiguration configuration = new ConnectToUrlConfiguration();
            configuration.UseProxySettings = false;
            configuration.SendClientCertificate = false;

            IConnection connectToUrlConnection = new ConnectToUrl () { Configuration = configuration };

            LoadFromWebRequestConfiguration loadFromWebRequestConfiguration = new LoadFromWebRequestConfiguration();
            loadFromWebRequestConfiguration.Url = "http://jsonplaceholder.typicode.com/todos/3";
            loadFromWebRequestConfiguration.Method = "GET";

            IDataSource loadFromWebRequest = new LoadFromWebRequest() { Configuration = loadFromWebRequestConfiguration };
            
            IDatastore dataObject = new DataObject();
            loadFromWebRequest.LoadData(connectToUrlConnection, dataObject, Test_Helpers.ReportProgressMethod);
        }
        
    }
}
