using IntegrationTool.DBAccess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.ApplicationCore
{
    [TestClass]
    public class Test_SqliteWrapper
    {
        [TestMethod]
        public void Test_CreateAndDelete_Successful()
        {
            string tempPath = Path.GetTempPath();
            Guid dbName = Guid.NewGuid();
            SqliteWrapper sqliteWrapper = new SqliteWrapper(tempPath, dbName.ToString());
            sqliteWrapper.Dispose();
            sqliteWrapper.DeleteDatabase();
        }
        
    }
}
