using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DependencyGraphGenerator
{
    public class TcBuildConfiguration
    {
        public List<string> SnapshotDependency{ get; private set; }
        public bool IsVcsTriggered { get; private set; }
        public string Name { get; private set; }

        public TcBuildConfiguration(string id, string name, List<string> dependsOn, List<string> snapshotDependency, bool isVcsTriggered,               List<TcArtifacts> artifactsDependency)
        {
            Id = id;
            Name = name;
            DependsOn = dependsOn?? new List<string>();
            SnapshotDependency = snapshotDependency??new List<string>();
            ArtifactsDependency = artifactsDependency??new List<TcArtifacts>();
            IsVcsTriggered = isVcsTriggered;
        }

        public string Id { get;private set; }

        public List<string> DependsOn { get;private set; }

        public bool HasSnapshotDependency
        {
            get { return SnapshotDependency.Count > 0; }
        }

        public List<TcArtifacts> ArtifactsDependency { get; private set; }
    }
}