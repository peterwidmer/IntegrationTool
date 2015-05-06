using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.ApplicationCore;

namespace IntegrationTool.UnitTests.ApplicationCore
{
    [TestClass]
    public class Test_ModuleLoader
    {
        [TestMethod]
        public void LoadSourceModule()
        {
            ModuleLoader moduleLoader = new ModuleLoader();
            moduleLoader.LoadModules(@"Modules\Sources");

            if(moduleLoader.Modules.Count == 0)
            {
                Assert.Fail("No module was loaded");
            }
        }
    }
}
