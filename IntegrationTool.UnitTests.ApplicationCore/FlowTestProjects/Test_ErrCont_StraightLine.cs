using IntegrationTool.ApplicationCore;
using IntegrationTool.Flowmanagement;
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
    public class Test_ErrCont_StraightLine
    {
        [TestMethod]
        public void TestStraightLine()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorContinuation\StraightLine.xml", 
                                                        "StraightLinePackage");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[2].State == ItemEvent.StoppedNotExecuted);
            Assert.IsTrue(testExecutor.ItemStops[3].State == ItemEvent.StoppedNotExecuted);
        }

        [TestMethod]
        public void TestStraightLineFirstError()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorContinuation\StraightLine.xml",
                                                        "StraightLineFirstError");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedNotExecuted);
            Assert.IsTrue(testExecutor.ItemStops[2].State == ItemEvent.StoppedNotExecuted);
        }

        [TestMethod]
        public void TestStraightLineContinue()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorContinuation\StraightLine.xml",
                                                        "StraightLinePackageContinue");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[2].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[3].State == ItemEvent.StoppedSuccessful);
        }

        [TestMethod]
        public void TestStraightLineSideError()
        {
            TestPackageExecutor testExecutor = TestPackageExecutor.ExecuteProjectPackage(
                                                        @"FlowTestProjects\ErrorContinuation\StraightLine.xml",
                                                        "StraightLineSideError");

            Assert.IsTrue(testExecutor.ItemStops[0].State == ItemEvent.StoppedWithError);
            Assert.IsTrue(testExecutor.ItemStops[1].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[2].State == ItemEvent.StoppedSuccessful);
            Assert.IsTrue(testExecutor.ItemStops[3].State == ItemEvent.StoppedSuccessful);
        }
    }
}
