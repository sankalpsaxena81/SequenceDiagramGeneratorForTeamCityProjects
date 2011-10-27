using System;
using System.Collections.Generic;
using System.IO;
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
             GetSequenceDiagramAsStringForEachProjects();
             files.ForEach(f =>
                               {

                                   hasSavedAllFiles&=f.SaveFile(path);
                               });
            return hasSavedAllFiles;
        }

        private void DeleteExstingFilesInDirectory(string path)
        {
            var files = Directory.GetFiles(path);
            files.ForEach(f=>File.Delete(f));
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

        //This is a recursive method that will keep on driling a build 
        // configuration one after the other in the workflow
        private void DrillDependency(TcBuildConfiguration drillPoint, TcProjects tcProjects)
        {
            var dependentBuilds = tcProjects.FindAllBuildConfigurationsDependentOnBuildConfigurationId(drillPoint.Id);
            drillPoint.SnapshotDependency.ForEach(sd =>
            {
                var desc = String.Format("{0} will wait for this to complete successfully",drillPoint.Name);
                finalString += FormatString(tcProjects, drillPoint, tcProjects.FindBuildConfigurationById(sd),desc);
            });

            if (drillPoint.HasArtifactsDependency)
            {
                drillPoint.ArtifactsDependency.ForEach(ad =>
                {
                    var desc = string.Format("Get {0} artifacts", ad.ArtifactFile);
                    finalString += FormatStringForArtifacts(tcProjects, drillPoint, tcProjects.FindBuildConfigurationById
                                    (ad.BuildConfigurationId), desc);
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
}