using System.Collections.Generic;

namespace DependencyGraphGenerator
{
    public class TcProject
    {
        public string Id;

        public TcProject(string id)
        {
            Id = id;
            BuildConfigurations= new List<TcBuildConfiguration>();
        }

        public List<TcBuildConfiguration> BuildConfigurations { get; set; }
        public string ProjectName { get; set; }
    }
}