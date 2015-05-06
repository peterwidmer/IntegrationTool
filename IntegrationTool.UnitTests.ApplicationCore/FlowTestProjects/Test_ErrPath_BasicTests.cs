using IntegrationTool.SDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.UnitTests.ApplicationCore.FlowTestProjects
{
    [TestClass]
    public class Test_ErrPath_BasicTests
    {
        [TestMethod]
        public void BasicTest_OneItemOnErrorPath()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorPath\SimpleErrorPathTests.xml",
                                                        "BasicTest_OneItemOnErrorPath");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedSuccessful);
        }
    }
}
