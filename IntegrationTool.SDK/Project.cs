using IntegrationTool.ApplicationCore;
using IntegrationTool.ApplicationCore.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTool.SDK
{
    public class Project
    {
        /// <summary>
        /// Display-Name of the project
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Physical path where the project-configuration is stored
        /// </summary>
        public string ProjectFolder { get; set; }

        /// <summary>
        /// Packages are container for flow-steps
        /// </summary>
        public ObservableCollection<Package> Packages { get; set; }

        [XmlIgnore]
        public ObservableCollection<RunLog> RunLogs { get; set; }

        public ObservableCollection<ConnectionConfigurationBase> Connections { get; set; }

        public Project()
        {
            Packages = new ObservableCollection<Package>();
            RunLogs = new ObservableCollection<RunLog>();
            Connections = new ObservableCollection<ConnectionConfigurationBase>();
        }

        public void Initialize(List<ModuleDescription> modules)
        {
            // Set the connection-types again, as they can be serialized only as string
            foreach (ConnectionConfigurationBase connectionConfiguration in this.Connections)
            {
                ModuleDescription connectionModuleDescription = modules.Where(t => t.ModuleType.AssemblyQualifiedName == connectionConfiguration.ConnectionTypeName).FirstOrDefault();
                connectionConfiguration.ModuleDescription = connectionModuleDescription;
            }
            foreach (Package package in this.Packages)
            {
                package.ParentProject = this;
            }
        }

        /// <summary>
        /// Load all runlogs
        /// </summary>
        public void LoadRunLogs()
        {
            this.RunLogs.Clear();
            string logBasePath = this.ProjectFolder + @"\logs\";
            if (Directory.Exists(logBasePath))
            {
                foreach (string runLogFileDirectory in Directory.GetDirectories(logBasePath))
                {
                    if(File.Exists(runLogFileDirectory + @"\runLog.xml") == false)
                    {
                        continue;
                    }

                    string serializedRunLog = ConfigurationFileHandler.ReadStringFromFile(runLogFileDirectory + @"\runLog.xml");
                    RunLog runLog = (RunLog)ConfigurationSerializer.DeserializeObject(serializedRunLog, typeof(RunLog), new Type[] { });
                    this.RunLogs.Add(runLog);
                }
            }
        }

        public static Project LoadFromFile(string projectPath, Type[] extraTypes)
        {
            string serializedProject = ConfigurationFileHandler.ReadStringFromFile(projectPath);
            Project project = (Project)ConfigurationSerializer.DeserializeObject(serializedProject, typeof(Project), extraTypes);
            project.ProjectFolder = projectPath.Replace(Path.GetFileName(projectPath), "");

            string connectionsPath = project.ProjectFolder + "\\" + project.ProjectName + "Connections.xml";
            if (File.Exists(connectionsPath))
            {
                string serializedConnections = ConfigurationFileHandler.ReadStringFromFile(connectionsPath);
                project.Connections = (ObservableCollection<ConnectionConfigurationBase>)ConfigurationSerializer.DeserializeObject(serializedConnections, typeof(ObservableCollection<ConnectionConfigurationBase>), extraTypes);
            }
            return project;
        }

        
    }
}
