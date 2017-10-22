using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.ProjectDesigner.Classes;
using System.IO;

namespace IntegrationTool.UnitTests.ProjectDesigner
{
    [TestClass]
    public class Test_RecentFilesList
    {
        [TestMethod]
        public void Test_RecentFilesList_LifeCycle()
        {
            string recentFilesPath = ApplicationLocalStorageHelper.GetApplicationDataFilePath(RecentFilesList.RECENTFILESSTORENAME);
            if(File.Exists(recentFilesPath))
            {
                File.Delete(recentFilesPath);
            }

            var recentFilesList1 = new RecentFilesList();
            recentFilesList1.Add(@"c:\temp\test1");

            var recentFilesList2 = new RecentFilesList();
            Assert.AreEqual(1, recentFilesList2.RecentFiles.Count);
        }
    }
}
