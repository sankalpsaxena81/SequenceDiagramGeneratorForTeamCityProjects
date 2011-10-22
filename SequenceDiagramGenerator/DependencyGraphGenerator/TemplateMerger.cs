using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Common;

namespace DependencyGraphGenerator
{
    public class TemplateMerger
    {
        private readonly Dictionary<string, XElement> _dictionary;
        private readonly string _rootFolder;

        public TemplateMerger(Dictionary<string, XElement> dictionary, string rootFolder)
        {
            _dictionary = dictionary;
            _rootFolder = rootFolder;
        }

        public Dictionary<string, string> MergeAllProjectConfigs()
        {
            var dictionary = new Dictionary<string, string>();
            var projectFolders = DirectoryAndFileOperations.GetAllTcProjectFolders(_rootFolder);
            projectFolders.ForEach(pf =>
                                       {
                                           var configAsXElement =
                                               DirectoryAndFileOperations.ReadConfigAsXElement(pf);
                                           var mergedConfig  = MergeTemplatesWithConfig(configAsXElement);
                                           dictionary.Add(pf, mergedConfig.ToString());
                                       });
            return dictionary;
        }

        public XElement MergeTemplatesWithConfig(XElement configAsXElement)
        {
            var newConfifAsXElement = new XElement(configAsXElement);
            newConfifAsXElement.RemoveNodes();

            configAsXElement.Elements().ForEach(e=>
                                                    {
                                                        if (e.Name.LocalName != "build-type")
                                                        {
                                                            newConfifAsXElement.Add(e);
                                                        }
                                                    });

            var buildTypes = configAsXElement.Elements("build-type");
            buildTypes.ForEach(bt=>
                                   {
                                       var btElement = new XElement(bt);
                                       btElement.RemoveNodes();
                                       bt.Elements().ForEach(bte=> { if(bte.Name.LocalName!="settings")btElement.Add(bte);});
                                       var settings = bt.Element("settings");
                                       var template = settings.AttributeValueOrDefault("ref",null);
                                       if(template!=null)
                                       {
                                           if(_dictionary.ContainsKey(template))
                                           {
                                               var mergedSettingsXElement = Merge(settings, _dictionary[template]);
                                               btElement.Add(mergedSettingsXElement);
                                           }
                                           else
                                            throw new Exception("Could not find template "+template);
                                       }
                                       else
                                           btElement.Add(settings);
                                      
                                       newConfifAsXElement.Add(btElement);
                                   }
                );
            return newConfifAsXElement;

        }

        private XElement Merge(XElement config, XElement template)
        {
            var mergedXelement = new XElement("settings");
            var settingsOfTemplate = template.Element("settings");
            settingsOfTemplate.Elements().ForEach(tempEl =>
                {
                    // copy existing template element
                    var newElement = new XElement(tempEl);
                    //find same element in config
                    var xelementFromConfig = GetXelementFromConfig(config,tempEl);
                    //if found merge
                    if(xelementFromConfig!=null && xelementFromConfig.Elements().ToList().Count>0)
                    {
                        MergeXElements(newElement, xelementFromConfig);
                    }
                    mergedXelement.Add(newElement);
                });
            // now add all elements in config but not in template
            foreach (var element in config.Elements())
            {
                bool found = false;
                foreach (var sot in settingsOfTemplate.Elements())
                {
                    if(sot.Name.LocalName.Equals(element.Name.LocalName))
                    {
                        found = true;
                    }
                    if(!found)
                    {
                       mergedXelement.Add(element); 
                    }
                }
            }
            return mergedXelement;
        }

        private XElement MergeXElements(XElement newElement, XElement tce)
        {
            tce.Elements().ForEach(t =>newElement.Add(t));
            return newElement;
        }

        private XElement GetXelementFromConfig(XElement config, XElement elementToFind)
        {
            foreach (var element in config.Elements())
            {
                if (element.Name.LocalName.Equals(elementToFind.Name.LocalName))
                    return element;
            }
            return null;
        }
    }
}