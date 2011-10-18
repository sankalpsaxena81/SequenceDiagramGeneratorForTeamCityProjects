using System;
using System.Collections.Generic;

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
    }
}