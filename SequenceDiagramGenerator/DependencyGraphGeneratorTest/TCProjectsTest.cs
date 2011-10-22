using System.Collections.Generic;
using DependencyGraphGenerator;
using NUnit.Framework;

namespace DependencyGraphGeneratorTest
{
    [TestFixture]
    public class TcObjectBuilderTest
    {
        [Test]
        public void ShouldCreate3TeamCityProjects()
        {
            // method under test
            var tcProjects = new TcProjects();
            tcProjects.Add(new TcProject("Project1"));
            tcProjects.Add(new TcProject("Something2"));
            tcProjects.Add(new TcProject("Project3"));
            tcProjects.Add(new TcProject("Project4"));
            tcProjects.Add(new TcProject("Something5"));
            Assert.AreEqual(3,tcProjects.FindAllProjectsWithNameContaining("Project").Count);
        }

        [Test]
        public void ShouldGetProjectGivenABuildConfigurationId()
        {
            var tcProjects = new TcProjects();
            var tcProject1 = new TcProject("Project1");
            var tcProject2 = new TcProject("Project2");
            var tcProject3 = new TcProject("Project3");
            
            
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC1","BC1",null,null, false,null));
            tcProject1.BuildConfigurations.Add(new TcBuildConfiguration("BC2","BC2",null,null, false,null));
            tcProject2.BuildConfigurations.Add(new TcBuildConfiguration("BC3","BC3",null,null, false,null));
            tcProject3.BuildConfigurations.Add(new TcBuildConfiguration("BC4","BC4",null,null, false,null));
            tcProjects.Add(tcProject1);
            tcProjects.Add(tcProject2);
            tcProjects.Add(tcProject3);
            // method under test
            var project = tcProjects.FindProjectByBuildConfigurationId("BC3");
            Assert.IsNotNull(project);
            Assert.AreEqual(tcProject2,project);
        }
    }
}