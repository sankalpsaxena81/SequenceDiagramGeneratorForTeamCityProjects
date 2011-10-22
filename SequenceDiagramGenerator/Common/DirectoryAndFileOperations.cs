using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Common
{
    public class DirectoryAndFileOperations
    {
        public static List<string> GetAllTcProjectFolders(string rootFolder)
        {
            var directories = Directory.GetDirectories(rootFolder);
            return directories.ToList<string>().FindAll(d => !Path.GetFileName(d).StartsWith("_")); 
        }

        public static string ReadConfigAsString(string folderName)
        {
           return XDocument.Load(folderName + "\\project-config.xml").ToString();
        }

        public static XElement ReadConfigAsXElement(string folderName)
        {
            var configAsString = ReadConfigAsString(folderName);
            return XElement.Parse(configAsString);
        }
    }
}