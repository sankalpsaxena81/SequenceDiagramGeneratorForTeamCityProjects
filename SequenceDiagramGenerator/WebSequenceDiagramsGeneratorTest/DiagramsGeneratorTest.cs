using System;
using System.Collections.Generic;
using DependencyGraphGenerator;
using Moq;
using NUnit.Framework;
using WebSequenceDiagramsGenerator;

namespace WebSequenceDiagramsGeneratorTest
{
    [TestFixture]
    public class DiagramsGeneratorTest
    {

        
        [Test]
        public void ShouldGenerateOutputForWebSequenceDiagram()
        {
            //setup
            var path = "C:\\Users\\ssaxena\\.BuildServer\\config";
            var tcProjects = new TcProjects();
            var tcProject1 = new TcProject("Project1");
            var blankList = new List<string>();
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC1", "Build", blankList, blankList, true,null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC2", "UnitTest", new List<string> { "BC1"}, blankList, false,null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC3", "Artifacts", new List<string> { "BC2" }, new List<string> { "BC4", "BC5" }, false, null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC4", "ContractTest1", blankList, blankList, false, null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC5", "ContractTest2", blankList, blankList, false, null));
            tcProjects.Add(tcProject1);

//            var objectBuilder = new Mock<ITcObjectBuilder>(MockBehavior.Strict);

//            objectBuilder.Setup(ob => ob.Parse(path)).Returns(tcProjects);

            // method under test
            
//            var diagramsGenerator = new DiagramsGenerator(objectBuilder.Object, path);
            var diagramsGenerator = new DiagramsGenerator(new TcObjectBuilder(), path);
            var dependencyString = diagramsGenerator.GetDependencyStringForAllProjects();
           
//            objectBuilder.VerifyAll();
            
            Assert.IsNotNull(dependencyString);
            Assert.AreEqual("Build=>UnitTest: /n UnitTest=>Artifacts: /n Artifacts=>ContractTest1: /n Artifacts=>ContractTest2: /n ", dependencyString);
        }

        [Test]
        public void ShouldGenerateOutputForWebSequenceDiagramAndSaveToFile()
        {
            //setup
            var path = "C:\\Users\\ssaxena\\Desktop\\temp\\New folder\\config";
            var tcProjects = new TcProjects();
            var tcProject1 = new TcProject("Project1");
            var blankList = new List<string>();
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC1", "Build", blankList, blankList, true, null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC2", "UnitTest", new List<string> { "BC1" }, blankList, false, null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC3", "Artifacts", new List<string> { "BC2" }, new List<string> { "BC4", "BC5" }, false, null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC4", "ContractTest1", blankList, blankList, false, null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC5", "ContractTest2", blankList, blankList, false, null));
            tcProjects.Add(tcProject1);

            //            var objectBuilder = new Mock<ITcObjectBuilder>(MockBehavior.Strict);

            //            objectBuilder.Setup(ob => ob.Parse(path)).Returns(tcProjects);

            // method under test

            //            var diagramsGenerator = new DiagramsGenerator(objectBuilder.Object, path);
            var diagramsGenerator = new DiagramsGenerator(new TcObjectBuilder(), path);
            var hasSaved =
                diagramsGenerator.GenerateDiagramsAndSaveAsFilesInDirectory(
                    "C:\\Users\\ssaxena\\Desktop\\AutoSequenceDiagramGenerator");

            //            objectBuilder.VerifyAll();

            Assert.IsTrue(hasSaved);
            
        }
    }
}