using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DependencyGraphGenerator
{
    public class TcProjects:List<TcProject>
    {
        public TcProjects FindAllProjectsWithNameContaining(string substring)
        {
            var tcProjects = new TcProjects();
            tcProjects.AddRange(this.FindAll(p =>p.Id.Contains(substring)));
            return tcProjects;
        }


        public TcProject FindProjectByBuildConfigurationId(string id)
        {
            TcProject project = null;
        
            ForEach(p=>p.BuildConfigurations.ForEach(bc=>
                                                         {
                                                           if (bc.Id.Equals(id))
                                                               project = p;
                                                         } ));
            return project;
        }

        public TcBuildConfiguration FindBuildConfigurationById(string id)
        {
            TcBuildConfiguration buildConfiguration = null;

            ForEach(p => p.BuildConfigurations.ForEach(bc =>
            {
                if (bc.Id.Equals(id))
                    buildConfiguration = bc;
            }));
            return buildConfiguration;
        }

        public List<TcBuildConfiguration> FindAllBuildConfigurationsDependentOnBuildConfigurationId(string id)
        {
            var list = new List<TcBuildConfiguration>();
            ForEach(p => p.BuildConfigurations.ForEach(bc =>
                    {
                        if(bc.DependsOn.Contains(id))
                            list.Add(bc);           
                    }));
            return list;
        }

        public List<TcBuildConfiguration> FindBuildConfigurationcDependentOnBuildConfiguration(string id)
        {
            var list = new List<TcBuildConfiguration>();
            ForEach(p => p.BuildConfigurations.ForEach(bc =>
                    {
                        
                        if (bc.HasArtifactsDependency)
                            FindInArtifactsDependency(bc, id, list);
                        if (bc.HasSnapshotDependency)
                            FindInSnapshotDependency(bc, id, list);
                        
                    }));
            return list;
        }

        private void FindInSnapshotDependency(TcBuildConfiguration bc, string id, List<TcBuildConfiguration> list)
        {
            bc.SnapshotDependency.ForEach(sd =>
                                              {
                                                  if(sd.Equals(id))   
                                                      list.Add(bc);
                                              });
        }

        private void FindInArtifactsDependency(TcBuildConfiguration bc, string id, List<TcBuildConfiguration> list)
        {
            bc.ArtifactsDependency.ForEach(ad =>
                                               {
                                                   if(ad.BuildConfigurationId.Equals(id) )
                                                       list.Add(bc);  
                                               });
        }
    }
}