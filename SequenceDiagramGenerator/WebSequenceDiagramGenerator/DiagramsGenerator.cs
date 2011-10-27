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
        private List<OutputFile> files;
        private TcProjects tcProjects;


        public DiagramsGenerator(ITcObjectBuilder objectBuilder, string configPath)
        {
            _objectBuilder = objectBuilder;
            _configPath = configPath;
            dictionary = new Dictionary<string, string>();
            files= new List<OutputFile>();
        }

        public Dictionary<string, string> GetSequenceDiagramAsStringForEachProjects()
        {
            tcProjects = _objectBuilder.BuildObjectGraph(_configPath);
            tcProjects.ForEach(tcp =>
                                   {
                                       dictionary.Add(tcp.Id, GenerateDependencyString(tcp.Id, tcProjects));
                                       

                                   });
            return dictionary;
        }

        public bool GenerateDiagramsAndSaveAsFilesInDirectory(string path)
        {
            bool hasSavedAllFiles = true;
            DeleteExstingFilesInDirectory(path);
            var ds = GetSequenceDiagramAsStringForEachProjects();
//            ds.Keys.ForEach(d=>
//                                {
//                                    if (ds[d] == string.Empty)
//                                        return;
//                                    hasSavesAllFiles=CreateHtmlFileWithSequenceDiagram(ds, d, path);
//                                });
            files.ForEach(f =>
                              {
                                  var content = CreateHtmlFileWithSequenceDiagram(f);
                                  hasSavedAllFiles&=WriteLinesToFile(f, content, path);
                              });
            return hasSavedAllFiles;
        }

        private string[] CreateHtmlFileWithSequenceDiagram(OutputFile d)
        {
            
            var stringToBeWritten = GetStringToBeWritten(d.FinalString);
            string[] lines = new string[1];
            lines[0]=CreateFirstLine();
            lines = lines.Concat(CreateSequenceDiagramLines(stringToBeWritten)).ToArray();
            lines=lines.Concat(new string[1]{CreateLastLine()}).ToArray();
            return lines;
        }

        private bool WriteLinesToFile(OutputFile d, string[] lines, string path)
        {
            bool hasSavesAllFiles=true;
            try
            {
                File.WriteAllLines(string.Format("{0}\\{1}-{2}.html", path, d.ProjName.ProjectName,d.BuildConfName), lines);
            }
            catch (Exception)
            {

                hasSavesAllFiles=false;
            }
            return hasSavesAllFiles;
        }

        private string CreateLastLine()
        {
            return "</pre></div><script type=\"text/javascript\" src=\"http://www.websequencediagrams.com/service.js\"></script>";
        }

        private string[] CreateSequenceDiagramLines(string[] stringToBeWritten)
        {
            string[] lines = new string[stringToBeWritten.Length];
            int i = 0;
            foreach (var str in stringToBeWritten)
                lines[i++] = str;
            return lines;
        }

        private string CreateFirstLine()
        {
            return "<div class=wsd wsd_style=\"modern-blue\" ><pre>";
        }

        private void DeleteExstingFilesInDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            files.ForEach(f=>File.Delete(f));
        }

        private string[] GetStringToBeWritten(string s)
        {
            return s.Split('#');
        }

        private string GenerateDependencyString(string pd, TcProjects tcProjects)
        {
           
            var tcProject = tcProjects.Find(p => p.Id.Equals(pd));
            var startPoints = GetBuildConfigurationsToStartDrillingFrom(tcProject);
            if (startPoints.Count > 0)
                startPoints.ForEach(sp =>
                                        {
                                            finalString = string.Empty;
                                            DrillDependency(sp, tcProjects);
                                            files.Add(new OutputFile(tcProject, sp.Name, finalString));
                                        });
            
            return  finalString;
        }

        private List<TcBuildConfiguration> GetBuildConfigurationsToStartDrillingFrom(TcProject tcProject)
        {
            var list = new List<TcBuildConfiguration>();
            var buildConfigurations = tcProject.BuildConfigurations.FindAll(bc => bc.IsVcsTriggered || bc.IsPinnedConfiguration);
            buildConfigurations.ForEach(bc => list.Add(bc));
            return list;
        }

        private void DrillDependency(TcBuildConfiguration drillPoint, TcProjects tcProjects)
        {
            var dependentBuilds = tcProjects.FindAllBuildConfigurationsDependentOnBuildConfigurationId(drillPoint.Id);
            
            
            drillPoint.SnapshotDependency.ForEach(sd =>
            {
            
                finalString += FormatString(tcProjects, drillPoint, tcProjects.FindBuildConfigurationById(sd),String.Format("{0} will wait for this to complete successfully",drillPoint.Name)); 
            });

            if (drillPoint.HasArtifactsDependency)
            {
                drillPoint.ArtifactsDependency.ForEach(ad =>
                {
                    finalString += FormatStringForArtifacts(tcProjects, drillPoint, tcProjects.FindBuildConfigurationById(ad.BuildConfigurationId), string.Format("Get {0} artifacts", ad.ArtifactFile));

                });
            }

            if (drillPoint.HasSnapshotDependency)
            {
                finalString += FormatString(tcProjects, drillPoint, drillPoint, string.Format("After successful completion of all, {0} will complete",drillPoint.Name));
            }

            

            if (dependentBuilds.Count != 0)
                dependentBuilds.ForEach(db =>
                {
                    finalString += FormatString(tcProjects, drillPoint, db,
                                                "On completion of " + drillPoint.Name);
                });

            if (dependentBuilds.Count != 0)
                dependentBuilds.ForEach(db => DrillDependency(db, tcProjects));
            
            return ;
        }

        private string FormatString(TcProjects tcProjects, TcBuildConfiguration drillPoint, TcBuildConfiguration db,string desc)
        {
            return string.Format("{0}-{1}->{2}-{3}:{4}#", tcProjects.FindProjectByBuildConfigurationId(drillPoint.Id).ProjectName, drillPoint.Name, tcProjects.FindProjectByBuildConfigurationId(db.Id).ProjectName, db.Name, desc??string.Empty);
        }
        private string FormatStringForArtifacts(TcProjects tcProjects, TcBuildConfiguration drillPoint, TcBuildConfiguration db, string desc)
        {
            return string.Format("{0}-{1}-->{2}-{3}:{4}#", tcProjects.FindProjectByBuildConfigurationId(drillPoint.Id).ProjectName, drillPoint.Name, tcProjects.FindProjectByBuildConfigurationId(db.Id).ProjectName, db.Name, desc ?? string.Empty);
        }
    }

    public class OutputFile
    {
        public TcProject ProjName { get; private set; }
        public string BuildConfName { get;private  set; }
        public string FinalString { get; private set; }



        public OutputFile(TcProject projName, string buildConfName, string finalString)
        {
            ProjName = projName;
            BuildConfName = buildConfName;
            FinalString = finalString;
        }
    }
}