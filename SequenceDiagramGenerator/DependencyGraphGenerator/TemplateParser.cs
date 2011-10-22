using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Common;

namespace DependencyGraphGenerator
{
    public class TemplateParser
    {
        private readonly string _rootFolder;

        public TemplateParser(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public Dictionary<string, XElement> GetAllTemplates()
        {
            var xElements = new Dictionary<string, XElement>();
            var allTcProjectFolders = DirectoryAndFileOperations.GetAllTcProjectFolders(_rootFolder);
            allTcProjectFolders.ForEach(pf =>
                                            {
                                                var xml = DirectoryAndFileOperations.ReadConfigAsString(pf);
                                                var dictionary = Parse(xml);
                                                dictionary.Keys.ForEach(k=>xElements.Add(k,dictionary[k]));
                                            });
            return xElements;
        }

        public Dictionary<string, XElement> Parse(string xml)
        {
            var xElements = new Dictionary<string, XElement>();
            var xElement = XElement.Parse(xml);
            var templates = xElement.Elements("template");
            templates.ForEach(t=> xElements.Add(t.AttributeValue("id"),t));
            return xElements;
        }
    }
}