using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Common;

namespace DependencyGraphGenerator
{
    public class TcObjectBuilder : ITcObjectBuilder
    {
        public TcObjectBuilder()
        {
        }

        public  TcProjects Parse(string rootFolder)
        {
            var tcProjects = new TcProjects();
            var projectDirectories = DirectoryAndFileOperations.GetAllTcProjectFolders(rootFolder);
            projectDirectories.ForEach(pd=>tcProjects.Add(CreateProject(pd)));
            return tcProjects;
        }

        private static TcProject CreateProject(string pd)
        {
            var xml = DirectoryAndFileOperations.ReadConfigAsString(pd);
            var tcProject = new ProjectFileParser().Parse(xml);
            return tcProject;
           
        }

        public TcProjects Parse(Dictionary<string, string> projectConfigs)
        {
            var tcProjects = new TcProjects();
            projectConfigs.Keys.ForEach(pc => tcProjects.Add(new ProjectFileParser().Parse(projectConfigs[pc])));
            return tcProjects;
        }
    }
}