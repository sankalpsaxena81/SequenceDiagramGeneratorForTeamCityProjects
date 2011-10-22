using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DependencyGraphGenerator;
using Moq;
using NUnit.Framework;
using WebSequenceDiagramsGenerator;

namespace DependencyGraphGeneratorTest.FunctionalTest
{
    [TestFixture]
    public class FunctionalTest1
    {

        [Test]
        public void ShouldTestEndToEnd()
        {
            string path = "C:\\Projects\\personal\\SequenceDiagramGeneratorForTeamCityProjects\\SequenceDiagramGenerator\\DependencyGraphGeneratorTest\\TestData\\config";

            var templateParser = new TemplateParser(path);
            // method under test
            var allTemplates = templateParser.GetAllTemplates();
            
            var templateMerger = new TemplateMerger(allTemplates, path);
            var mergeAllProjectConfigs = templateMerger.MergeAllProjectConfigs();

            var tcObjectBuilder = new TcObjectBuilder();
            var tcProjects = tcObjectBuilder.Parse(mergeAllProjectConfigs);

            var objectBuilder = new Mock<ITcObjectBuilder>(MockBehavior.Strict);

            objectBuilder.Setup(ob => ob.Parse("path")).Returns(tcProjects);
            var diagramsGenerator = new DiagramsGenerator(objectBuilder.Object, "path");
            var hasSaved = diagramsGenerator.GenerateDiagramsAndSaveAsFilesInDirectory(
                "C:\\Users\\ssaxena\\Desktop\\AutoSequenceDiagramGenerator");
            Assert.IsTrue(hasSaved);
        }
    }
}