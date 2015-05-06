using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IntegrationTool.SDK;
using IntegrationTool.ApplicationCore.Serialization;

namespace IntegrationTool.UnitTests.ApplicationCore
{
    [TestClass]
    public class Test_ConfigurationHandling
    {
        [TestMethod]
        public void ConfigurationSerialization()
        {
            string projectName = "Serialization    Project";
            Project project = new Project()
            {
                ProjectName = "Serialiazation    Project",
                ProjectFolder = @"C:\temp"
            };

            string projectString = ConfigurationSerializer.SerializeObject(project, new Type [] {});
            Project deserializedProject = (Project)ConfigurationSerializer.DeserializeObject(projectString, typeof(Project), new Type[] { });

            StringAssert.Equals(deserializedProject.ProjectName, projectName);
        }
    }
}
