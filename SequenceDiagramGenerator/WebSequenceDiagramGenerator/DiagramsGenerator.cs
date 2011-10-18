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

        public Dictionary<string, string> GetDependencyString()
        {
            var tcProjects = _objectBuilder.Parse(_configPath);
            tcProjects.ForEach(tcp => dictionary.Add(tcp.Id, GenerateDependencyString(tcp.Id,tcProjects)));
            return dictionary;
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
                dependentBuilds.ForEach(db =>{finalString += FormatString(tcProjects, drillPoint, db);});
            
            drillPoint.SnapshotDependency.ForEach(sd =>
            {
                finalString += FormatString(tcProjects, drillPoint, tcProjects.FindBuildConfigurationById(sd)); 
            });
            
            if (dependentBuilds.Count != 0)
                dependentBuilds.ForEach(db => DrillDependency(db, tcProjects));
            
            return ;
        }

        private string FormatString(TcProjects tcProjects, TcBuildConfiguration drillPoint, TcBuildConfiguration db)
        {
            return string.Format("{0}-{1}->{2}-{3}: /n ", tcProjects.FindProjectByBuildConfigurationId(drillPoint.Id).Id,drillPoint.Name, tcProjects.FindProjectByBuildConfigurationId(db.Id).Id,db.Name);
        }
    }
}