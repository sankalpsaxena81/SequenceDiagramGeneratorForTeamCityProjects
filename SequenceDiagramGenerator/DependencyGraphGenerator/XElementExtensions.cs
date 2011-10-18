using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DependencyGraphGenerator
{
    public static class XElementExtensions
    {
        public static string ElementValueOrEmpty(this XElement xElement, string elementName)
        {
            return xElement.ElementValueOrDefault(elementName, string.Empty);
        }

        public static string ElementValueOrDefault(this XElement xElement, string elementName, string defaultValue)
        {
            var child = xElement.ElementWithDefaultNameSpace(elementName);
            return child == null ? defaultValue : child.Value;
        }

        public static string AttributeValue(this XElement element, XName attributeName)
        {
            if (element == null) return null;
            var attribute = element.Attribute(attributeName);
            return attribute == null ? null : attribute.Value;
        }

        public static string AttributeValueOrDefault(this XElement xElement, string attributeName, string defaultValue)
        {
            var attribute = xElement.Attribute(attributeName);
            return attribute == null ? defaultValue : attribute.Value;
        }

        public static string AttributeValueOrEmpty(this XElement xElement, string attributeName)
        {
            return xElement.AttributeValueOrDefault(attributeName, string.Empty);
        }

        public static DateTime? AttributeValueAsDate(this XElement xElement, string attributeName)
        {
            var date = xElement.AttributeValueOrDefault(attributeName, string.Empty);
            DateTime result;
            return (DateTime.TryParse(date, out result)) ? result : (DateTime?)null;
        }

        public static DateTime? ElementValueAsDate(this XElement xElement, string elementName)
        {
            var date = xElement.ElementValueOrEmpty(elementName);
            DateTime result;
            return (DateTime.TryParse(date, out result)) ? result : (DateTime?)null;
        }

        public static IEnumerable<XElement> ElementsWithDefaultNameSpace(this XElement xElement, string elementName)
        {
            return xElement.Elements(xElement.GetDefaultNamespace() + elementName);
        }

        public static XElement ElementWithDefaultNameSpace(this XElement xElement, string elementName)
        {
            return xElement.Element(xElement.GetDefaultNamespace() + elementName);
        }

        public static string ElementContentWithDefaultNamespace(this XElement xElement, string name)
        {
            var element = xElement.Element(xElement.GetDefaultNamespace() + name);
            return element == null ? string.Empty : element.ToString();
        }

        public static string XPathSelectElementValueOrEmpty(this XElement xElement, string xpath, IXmlNamespaceResolver namespaceResolver)
        {
            var child = xElement.XPathSelectElement(xpath, namespaceResolver);
            return child == null ? string.Empty : child.Value;
        }
    }
}