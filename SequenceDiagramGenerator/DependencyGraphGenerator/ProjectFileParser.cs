using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DependencyGraphGenerator
{
    public class ProjectFileParser
    {
        private XElement xElement;

        public TcProject Parse(string xml)
        {
            
            xElement = XElement.Parse(xml);
            var id = xElement.AttributeValue("id");
            var tcProject = new TcProject(id);
            var xElements = xElement.Elements("build-type");
            xElements.ForEach(AddBuildConfigurationToProject(tcProject));
            return tcProject;
        }

        private Action<XElement> AddBuildConfigurationToProject(TcProject tcProject)
        {
            return x =>
                       {
                           var id = x.AttributeValueOrEmpty("id");
                           var name = x.AttributeValueOrEmpty("name");
                           var dependsOn = GetDependsOn(x);
                           var snapshotDependency = GetSnapshotDependency(x);
                           bool isVcsTriggered = IsVcsTriggered(x);
                           tcProject.BuildConfigurations.Add(
                             new TcBuildConfiguration(id, name, dependsOn, snapshotDependency,isVcsTriggered));
                       };
        }

        private bool IsVcsTriggered(XElement element)
        {
            bool isVcsTriggered = false;

            var settings = element.Element("settings");
            if (settings == null) return isVcsTriggered;
            var bt = settings.Elements("build-triggers");
            bt.ForEach(t =>
                           {
                               var buildTrigger = t.Element("build-trigger");
                               if (buildTrigger == null) return;
                               if (buildTrigger.AttributeValue("name").Equals("vcsTrigger"))
                               {
                                   isVcsTriggered=true;
                               }
                           });
            return isVcsTriggered;

        }

        private List<string> GetSnapshotDependency(XElement buildConfigurationElement)
        {
            var list = new List<string>();
            var settings = buildConfigurationElement.Element("settings");
            if (settings == null) return list;
            var dependencies = settings.Element("dependencies");
            if (dependencies == null) return list;
            var snapshotDependencies = dependencies.Elements("depend-on");
            snapshotDependencies.ForEach(sd => list.Add(sd.AttributeValue("sourceBuildTypeId")));
            return list;
        }

        private List<string> GetDependsOn(XElement buildConfigurationElement)
        {
            var list = new List<string>();
            var settings = buildConfigurationElement.Element("settings");
            if (settings == null) return list;
            var bt = settings.Element("build-triggers");
            if (bt == null) return list;
            var buildTrigger = bt.Elements("build-trigger");
            buildTrigger.ForEach(t =>
            {
                var parameters = t.Element("parameters");
                if (parameters == null) return;
                var @params = parameters.Elements("param");
                @params.ForEach(p =>
                    {
                        if (p.AttributeValueOrEmpty("name").Equals("dependsOn"))
                        {
                            list.Add(p.AttributeValueOrEmpty("value"));
                        }
                    });
            });
            return list;
        }
    }
}