using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTool.ApplicationCore
{
    public class ConfigurationFileHandler
    {
        public static void SaveStringToFile(string fileName, string filePath, string content)
        {
            Directory.CreateDirectory(filePath);
            if(filePath.EndsWith(@"\") == false)
            {
                filePath += @"\";
            }

            using(StreamWriter streamWriter = new StreamWriter(filePath + fileName))
            {
                streamWriter.Write(content);
            }
        }

        public static string ReadStringFromFile(string fullFilePath)
        {
            using (StreamReader streamReader = new StreamReader(fullFilePath))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
