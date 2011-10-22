using DependencyGraphGenerator;
using NUnit.Framework;

namespace DependencyGraphGeneratorTest
{
    [TestFixture]
    public class ProjectFileParserTest
    {
        [Test]
        public void ShouldReturnProjectWith2BuildConfigurations()
        {
            #region xml
            string xml = @"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE project SYSTEM '../project-config.dtd'>

<project id='project2'>
  <parameters />
  <build-type id='bt14' name='Artifacts'>
    <description />
    <settings>
      <parameters />
      <build-runners />
      <vcs-settings checkout-mode='ON_SERVER' labeling-type='NONE' labeling-pattern='build-%system.build.number%' />
      <requirements />
      <build-triggers>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt7' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <artifact-publishing paths='p1b.txt' />
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt6' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p1b.txt' />
        </dependency>
      </artifact-dependencies>
      <dependencies>
        <depend-on sourceBuildTypeId='bt12'>
          <options>
            <option name='take-started-build-with-same-revisions' value='true' />
            <option name='take-successful-builds-only' value='true' />
          </options>
        </depend-on>
        <depend-on sourceBuildTypeId='bt13'>
          <options>
            <option name='take-started-build-with-same-revisions' value='true' />
            <option name='take-successful-builds-only' value='true' />
          </options>
        </depend-on>
      </dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt6' name='Build'>
    <description />
    <settings>
      <parameters />
      <build-runners>
        <runner id='RUNNER_3' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='build.bat' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_SERVER' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='3' set-label='false' />
      </vcs-settings>
      <requirements />
      <build-triggers>
        <build-trigger name='vcsTrigger'>
          <parameters>
            <param name='quietPeriodMode' value='DO_NOT_USE' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <artifact-publishing paths='p1b.txt' />
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt12' name='P1ToP2ContractTest'>
    <description />
    <settings>
      <parameters />
      <build-runners>
        <runner id='RUNNER_3' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='ContractTestP1ToP2.bat' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_SERVER' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='3' set-label='false' />
      </vcs-settings>
      <requirements />
      <build-triggers />
      <artifact-publishing paths='p1u.txt&#xA;p1b.txt' />
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt6' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p1b.txt' />
        </dependency>
        <dependency sourceBuildTypeId='bt8' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p2b.txt' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt13' name='P1ToP3ContractTest'>
    <description />
    <settings>
      <parameters />
      <build-runners>
        <runner id='RUNNER_3' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='ContractTestP1ToP3.bat' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_SERVER' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='3' set-label='false' />
      </vcs-settings>
      <requirements />
      <build-triggers />
      <artifact-publishing paths='p1u.txt&#xA;p1b.txt' />
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt6' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p1b.txt' />
        </dependency>
        <dependency sourceBuildTypeId='bt10' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p3b.txt' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt7' name='UnitTest'>
    <description />
    <settings>
      <parameters />
      <build-runners>
        <runner id='RUNNER_3' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='unitTest.bat' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_SERVER' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='3' set-label='false' />
      </vcs-settings>
      <requirements />
      <build-triggers>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt6' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <artifact-publishing paths='p1u.txt&#xA;p1b.txt' />
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt6' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p1b.txt' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
</project>
";
#endregion
            // method under test
            var tcProject = new ProjectFileParser().Parse(xml);
           
            Assert.IsNotNull(tcProject);
            Assert.AreEqual(5,tcProject.BuildConfigurations.Count);
            Assert.AreEqual("project2",tcProject.Id);
            Assert.AreEqual("bt14", tcProject.BuildConfigurations[0].Id);
            Assert.AreEqual("Artifacts", tcProject.BuildConfigurations[0].Name);
            Assert.AreEqual(1, tcProject.BuildConfigurations[0].DependsOn.Count);
            Assert.AreEqual("bt7", tcProject.BuildConfigurations[0].DependsOn[0]);
            Assert.AreEqual(2, tcProject.BuildConfigurations[0].SnapshotDependency.Count);
            Assert.AreEqual("bt12", tcProject.BuildConfigurations[0].SnapshotDependency[0]);
            Assert.AreEqual("bt13", tcProject.BuildConfigurations[0].SnapshotDependency[1]);
        }

        [Test]
        public void ShouldReturnABuildConfigurationWith3DependsOn()
        {
            #region xml

            string xml = @"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE project SYSTEM '../project-config.dtd'>

<project id='project6'>
  <parameters />
  <build-type id='bt18' name='Artifacts'>
    <description />
    <settings>
      <parameters />
      <build-runners />
      <vcs-settings checkout-mode='ON_SERVER' labeling-type='NONE' labeling-pattern='build-%system.build.number%' />
      <requirements />
      <build-triggers>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt14' />
          </parameters>
        </build-trigger>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt15' />
          </parameters>
        </build-trigger>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt17' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <artifact-publishing paths='p1b.txt&#xA;p2b.txt&#xA;p3b.txt' />
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt14' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p1b.txt' />
        </dependency>
        <dependency sourceBuildTypeId='bt15' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p2b.txt' />
        </dependency>
        <dependency sourceBuildTypeId='bt17' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='p3b.txt' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
</project>";

            #endregion

            // method under test
            var tcProject = new ProjectFileParser().Parse(xml);
            
            Assert.IsNotNull(tcProject);
            Assert.AreEqual(1, tcProject.BuildConfigurations.Count);
            Assert.AreEqual("project6", tcProject.Id);
            Assert.AreEqual("bt18", tcProject.BuildConfigurations[0].Id);
            Assert.AreEqual("Artifacts", tcProject.BuildConfigurations[0].Name);
            Assert.AreEqual(3, tcProject.BuildConfigurations[0].DependsOn.Count);
            Assert.AreEqual(0, tcProject.BuildConfigurations[0].SnapshotDependency.Count);
            Assert.AreEqual(3, tcProject.BuildConfigurations[0].ArtifactsDependency.Count);
        }
    }
}