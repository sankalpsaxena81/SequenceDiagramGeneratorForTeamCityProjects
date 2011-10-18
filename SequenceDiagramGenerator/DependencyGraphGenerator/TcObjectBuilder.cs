using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DependencyGraphGenerator
{
    public class TcObjectBuilder : ITcObjectBuilder
    {
        public TcObjectBuilder()
        {
        }

        public  TcProjects Parse(string rootfolder)
        {
            var tcProjects = new TcProjects();
            var directories = Directory.GetDirectories(rootfolder);
            var projectDirectories = directories.ToList<string>().FindAll(d=>!Path.GetFileName(d).StartsWith("_"));
            projectDirectories.ForEach(pd=>tcProjects.Add(CreateProject(pd)));
            return tcProjects;
        }

        private static TcProject CreateProject(string pd)
        {

            var xDocument = XDocument.Load(pd + "\\project-config.xml");
            var tcProject = new ProjectFileParser().Parse(xDocument.ToString());
            return tcProject;
           
        }
    }
}