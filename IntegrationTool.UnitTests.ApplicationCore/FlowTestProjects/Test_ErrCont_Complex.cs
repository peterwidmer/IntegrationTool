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
    public class Test_ErrCont_Complex
    {
        [TestMethod]
        public void TestComplexTest1()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorContinuation\ComplexTests.xml",
                                                        "ComplexTest1");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[2].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[3].State == ItemEvent.StoppedNotExecuted);
            Assert.IsTrue(testExecutor.ItemStops[3].State == ItemEvent.StoppedNotExecuted);

        }

        [TestMethod]
        public void TestComplexTest2()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorContinuation\ComplexTests.xml",
                                                        "ComplexTest2");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[2].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[3].State == ItemEvent.StoppedNotExecuted);
            Assert.IsTrue(testExecutor.ItemStops[4].State == ItemEvent.StoppedSuccessful);

        }
    }
}
