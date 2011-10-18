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

        public TcBuildConfiguration(string id, string name, List<string> dependsOn, List<string> snapshotDependency, bool isVcsTriggered)
        {
            Id = id;
            Name = name;
            DependsOn = dependsOn;
            SnapshotDependency = snapshotDependency;
            IsVcsTriggered = isVcsTriggered;
        }

        public string Id { get;private set; }

        public List<string> DependsOn { get;private set; }

        public bool HasSnapshotDependency
        {
            get { return SnapshotDependency.Count > 0; }
        }
    }
}