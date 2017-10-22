using IntegrationTool.ApplicationCore.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ProjectDesigner.Classes
{
    public class RecentFilesList
    {
        public const string RECENTFILESSTORENAME = "RecentFilesList.txt";

        public ObservableCollection<string> RecentFiles { get; set; }

        public RecentFilesList()
        {
            RecentFiles = new ObservableCollection<string>();

            var serializedRecentFiles = ApplicationLocalStorageHelper.ReadFrom(RECENTFILESSTORENAME);
            if(!string.IsNullOrEmpty(serializedRecentFiles))
            {
                RecentFiles = (ObservableCollection<string>)ConfigurationSerializer.DeserializeObject(serializedRecentFiles, typeof(ObservableCollection<string>), new Type [] {});
            }            
        }

        public void Add(string file)
        {
            // Limit number of recent files to 5
            if(RecentFiles.Count > 4)
            {
                RecentFiles.RemoveAt(0);
            }

            if(RecentFiles.Contains(file))
            {
                RecentFiles.Remove(file);
            }

            RecentFiles.Add(file);

            var serializedRecentFiles = ConfigurationSerializer.SerializeObject(RecentFiles, new Type [] {});
            ApplicationLocalStorageHelper.WriteToFile(RECENTFILESSTORENAME, serializedRecentFiles);
        }
    }
}
