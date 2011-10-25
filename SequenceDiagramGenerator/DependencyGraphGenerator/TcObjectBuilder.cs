using System.Collections.Generic;
using System.Xml.Linq;

namespace DependencyGraphGenerator
{
    public class TcObjectBuilder : ITcObjectBuilder
    {
        public TcProjects BuildObjectGraph(string rootFolder)
        {
            var allTemplates = ExtractTemplates(rootFolder);
            var mergeAllProjectConfigs = MergeTemplatesWithConfigs(rootFolder, allTemplates);
            return CreateTCProjects(mergeAllProjectConfigs);
        }

        public TcProjects CreateTCProjects(Dictionary<string, string> projectConfigs)
        {
            var tcProjects = new TcProjects();
            projectConfigs.Keys.ForEach(pc => tcProjects.Add(new ProjectFileParser().Parse(projectConfigs[pc])));
            return tcProjects;
        }

        private Dictionary<string, string> MergeTemplatesWithConfigs(string rootFolder, Dictionary<string, XElement> allTemplates)
        {
            var templateMerger = new TemplateMerger(allTemplates, rootFolder);
            return templateMerger.MergeAllProjectConfigs();
        }

        private Dictionary<string, XElement> ExtractTemplates(string rootFolder)
        {
            var templateParser = new TemplateParser(rootFolder);
            return templateParser.GetAllTemplates();
        }

        

       
    }
}