using IntegrationTool.ApplicationCore.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntegrationTool.ProjectDesigner.Classes
{
    public class RecentFilesList
    {
        public const string RECENTFILESSTORENAME = "RecentFilesList.txt";

        public ObservableCollection<RecentFile> RecentFiles { get; set; }

        public RecentFilesList()
        {
            RecentFiles = new ObservableCollection<RecentFile>();

            var serializedRecentFiles = ApplicationLocalStorageHelper.ReadFrom(RECENTFILESSTORENAME);
            if(!string.IsNullOrEmpty(serializedRecentFiles))
            {
                RecentFiles = (ObservableCollection<RecentFile>)ConfigurationSerializer.DeserializeObject(serializedRecentFiles, typeof(ObservableCollection<RecentFile>), new Type [] {});
            }            
        }

        public void Add(string file)
        {
            // Limit number of recent files to 5
            if(RecentFiles.Count > 4)
            {
                RecentFiles.RemoveAt(0);
            }

            var newFile = new RecentFile()
            {
                FileName = Path.GetFileName(file),
                FullFilePath = file
            };

            var recentFile = RecentFiles.FirstOrDefault(t => t.FullFilePath == newFile.FullFilePath);
            if(recentFile != null)
            {
                RecentFiles.Remove(recentFile);
            }

            RecentFiles.Add(newFile);

            var serializedRecentFiles = ConfigurationSerializer.SerializeObject(RecentFiles, new Type [] {});
            ApplicationLocalStorageHelper.WriteToFile(RECENTFILESSTORENAME, serializedRecentFiles);
        }
    }
}
