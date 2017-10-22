using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ProjectDesigner.Classes
{
    public static class ApplicationLocalStorageHelper
    {
        public static void WriteToFile(string fileName, string content)
        {
            var fileToWritePath = GetApplicationDataFilePath(fileName);
            File.WriteAllText(fileToWritePath, content);
        }

        public static string ReadFrom(string fileName)
        {
            var fileToReadPath = GetApplicationDataFilePath(fileName);
            if (!File.Exists(fileToReadPath))
            {
                return null;
            }
            else
            {
                return File.ReadAllText(fileToReadPath);
            }
        }

        public static string GetApplicationDataFilePath(string fileName)
        {
            var applicationDataPath = GetApplicationDataPath();
            var fullApplicationDataFilePath = applicationDataPath + "\\" + fileName;
            return fullApplicationDataFilePath;
        }

        public static string GetApplicationDataPath()
        {
            string applicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\IntegrationTool";

            if (!Directory.Exists(applicationDataPath))
            {
                Directory.CreateDirectory(applicationDataPath);
            }

            return applicationDataPath;
        }
    }
}
