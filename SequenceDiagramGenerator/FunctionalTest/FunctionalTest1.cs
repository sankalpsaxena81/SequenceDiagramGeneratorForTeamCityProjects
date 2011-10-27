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
            string path = "C:\\Users\\ssaxena\\Desktop\\sankalpTCTemp\\config";
            var tcObjectBuilder = new TcObjectBuilder();
            var diagramsGenerator = new DiagramsGenerator(tcObjectBuilder, path);
            var hasSaved = diagramsGenerator.GenerateDiagramsAndSaveAsFilesInDirectory(
                "C:\\Users\\ssaxena\\Desktop\\AutoSequenceDiagramGenerator");
            Assert.IsTrue(hasSaved);
        }

        [Test]
        public void ShouldFindOutDependecy()
        {
            string path = "C:\\Users\\ssaxena\\Desktop\\sankalpTCTemp\\config";
            var tcObjectBuilder = new TcObjectBuilder();
            var tcProjects = tcObjectBuilder.BuildObjectGraph(path);
            

            var builds = tcProjects.FindBuildConfigurationcDependentOnBuildConfiguration("bt104");
        }
    }
}