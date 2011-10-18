using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DependencyGraphGenerator;

namespace WebSequenceDiagramsGenerator
{
    public class DiagramsGenerator
    {
        private readonly ITcObjectBuilder _objectBuilder;
        private readonly string _configPath;
        private string finalString=string.Empty;
        private Dictionary<string, string> dictionary;


        public DiagramsGenerator(ITcObjectBuilder objectBuilder, string configPath)
        {
            _objectBuilder = objectBuilder;
            _configPath = configPath;
            dictionary = new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetDependencyStringForAllProjects()
        {
            var tcProjects = _objectBuilder.Parse(_configPath);
            tcProjects.ForEach(tcp => dictionary.Add(tcp.Id, GenerateDependencyString(tcp.Id,tcProjects)));
            return dictionary;
        }

        public bool GenerateDiagramsAndSaveAsFilesInDirectory(string path)
        {
            bool hasSavesAllFiles = true;
            var ds = GetDependencyStringForAllProjects();
            ds.Keys.ForEach(d=>
                                {
                                    if (ds[d] != string.Empty)
                                    {
                                        var stringToBeWritten = GetStringToBeWritten(ds[d]);
                                        string[] lines = new string[stringToBeWritten.Length+2];
                                        lines[0] = "<div class=wsd wsd_style=\"modern-blue\" ><pre>";
                                        int i = 1;
                                        foreach (var str in stringToBeWritten)
                                        {
                                            lines[i++] = str;
                                        }
                                        lines[i] = "</pre></div><script type=\"text/javascript\" src=\"http://www.websequencediagrams.com/service.js\"></script>";
                                        try
                                        {
                                            File.WriteAllLines(string.Format("{0}\\{1}.html", path, d), lines);
                                        }
                                        catch (Exception)
                                        {

                                            hasSavesAllFiles=false;
                                        }
                                        
                                    }
                                });
            return hasSavesAllFiles;
        }

        private string[] GetStringToBeWritten(string s)
        {
            return s.Split('#');
        }

        private string GenerateDependencyString(string pd, TcProjects tcProjects)
        {
            finalString = string.Empty;
            var tcProject = tcProjects.Find(p => p.Id.Equals(pd));
            var startPoint = tcProject.BuildConfigurations.Find(bc => bc.IsVcsTriggered);
            if(startPoint!=null)
                DrillDependency(startPoint, tcProjects);
            return  finalString;
        }

        private void DrillDependency(TcBuildConfiguration drillPoint, TcProjects tcProjects)
        {
            var dependentBuilds = tcProjects.FindAllBuildConfigurationsDependentOnBuildConfigurationId(drillPoint.Id);
            if (dependentBuilds.Count != 0)
                dependentBuilds.ForEach(db =>{finalString += FormatString(tcProjects, drillPoint, db,null);});
            
            drillPoint.SnapshotDependency.ForEach(sd =>
            {
            
                finalString += FormatString(tcProjects, drillPoint, tcProjects.FindBuildConfigurationById(sd),drillPoint.Name+" will wait for this to complete successfully"); 
            });
            if (drillPoint.HasSnapshotDependency)
            {
                finalString += FormatString(tcProjects, drillPoint, drillPoint, "After successful completion of all");
            }

            if (dependentBuilds.Count != 0)
                dependentBuilds.ForEach(db => DrillDependency(db, tcProjects));
            
            return ;
        }

        private string FormatString(TcProjects tcProjects, TcBuildConfiguration drillPoint, TcBuildConfiguration db,string desc)
        {
            return string.Format("{0}-{1}->{2}-{3}:{4}#", tcProjects.FindProjectByBuildConfigurationId(drillPoint.Id).Id, drillPoint.Name, tcProjects.FindProjectByBuildConfigurationId(db.Id).Id, db.Name, desc??string.Empty);
        }
    }
}