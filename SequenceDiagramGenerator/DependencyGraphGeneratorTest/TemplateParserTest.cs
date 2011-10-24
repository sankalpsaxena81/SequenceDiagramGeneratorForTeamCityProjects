using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DependencyGraphGenerator;
using NUnit.Framework;

namespace DependencyGraphGeneratorTest
{
    [TestFixture]
    public class TemplateParserTest
    {
        [Test]
        public void ShouldExtraxt3TempletesFromXml()
        {
            #region xml
            string xml = @"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE project SYSTEM '../project-config.dtd'>

<project id='project4'>
  <parameters />
  <template id='btTemplate3' name='build'>
    <settings>
      <options>
        <option name='allowExternalStatus' value='true' />
        <option name='buildNumberPattern' value='%build.number.format%' />
        <option name='maximumNumberOfBuilds' value='1' />
      </options>
      <parameters>
        <param name='build.number.format' value='1.0.0.{0}' />
        <param name='checkout.dir' value='' />
      </parameters>
      <build-runners>
        <runner id='RUNNER_8' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='build.bat' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_AGENT' checkout-dir='%checkout.dir%' labeling-type='NONE' labeling-pattern='build-%system.build.number%' />
      <requirements>
        <equals name='system.os.name' value='Windows Server 2008 R2' />
        <does-not-contain name='system.agent.name' value='migration' />
      </requirements>
      <build-triggers>
        <build-trigger name='vcsTrigger'>
          <parameters>
            <param name='quietPeriodMode' value='DO_NOT_USE' />
            <param name='triggerRules' value='-:Tests.Contracts.*/**' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <build-extensions>
        <extension id='BUILD_EXT_3' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='build/Unit-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-publishing paths='build/*.zip&#xA;build/*.xml&#xA;build/*.html&#xA;build/package/poshsplice.yml' />
      <cleanup />
    </settings>
  </template>
  <template id='btTemplate7' name='coverage'>
    <settings>
      <options>
        <option name='buildNumberPattern' value='%build.number%' />
      </options>
      <parameters />
      <build-runners>
        <runner id='RUNNER_6' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='test.bat' />
            <param name='command.parameters' value='-testType Unit -Coverage true' />
            <param name='teamcity.build.workingDir' value='controller' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_AGENT' checkout-dir='%checkout.dir%' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='1' set-label='false'>
          <checkout-rule rule='+:BuildAndRelease/DeployScripts =&gt; .' />
        </vcs-entry-ref>
      </vcs-settings>
      <requirements />
      <build-triggers />
      <build-extensions>
        <extension id='BUILD_EXT_4' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='Unit-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-publishing paths='build/*.xml&#xA;build/*.html' />
      <cleanup />
    </settings>
  </template>
  <template id='btTemplate4' name='deploy / test'>
    <settings>
      <options>
        <option name='buildNumberPattern' value='%build.number%' />
        <option name='maximumNumberOfBuilds' value='1' />
      </options>
      <parameters>
        <param name='app' value='' />
        <param name='build.number' value='' />
        <param name='checkout.dir' value='' />
        <param name='dependency.build.number' value='%build.number%' />
        <param name='deploy.environment' value='functional-test-pie' />
        <param name='optional.parameters' value='' />
        <param name='test.type' value='Functional' />
      </parameters>
      <build-runners>
        <runner id='RUNNER_9' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='deploy.bat' />
            <param name='command.parameters' value='-env %deploy.environment% -app %app% -ver %dependency.build.number%' />
            <param name='teamcity.build.workingDir' value='controller' />
          </parameters>
        </runner>
        <runner id='RUNNER_10' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='controller\test.bat' />
            <param name='command.parameters' value='-env %deploy.environment% -testType %test.type% -app %app% %optional.parameters%' />
            <param name='teamcity.build.workingDir' value='controller' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_AGENT' checkout-dir='%checkout.dir%' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='1' set-label='false'>
          <checkout-rule rule='+:BuildAndRelease/DeployScripts =&gt; .' />
        </vcs-entry-ref>
      </vcs-settings>
      <requirements>
        <equals name='system.os.name' value='Windows Server 2008 R2' />
      </requirements>
      <build-triggers />
      <build-extensions>
        <extension id='BUILD_EXT_5' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-publishing paths='*.zip&#xA;poshsplice.yml&#xA;Tests.Contracts.*/*.log' />
      <cleanup>
        <keep cleanup-level='ARTIFACTS' builds='5' />
      </cleanup>
    </settings>
  </template>
  <build-type id='bt85' name='ais / contract test'>
    <description />
    <settings ref='btTemplate9'>
      <parameters>
        <param name='app' value='ais' />
        <param name='build.number' value='%dep.bt6.build.number%' />
        <param name='checkout.dir' value='cpe%app%deploycontractstest' />
        <param name='dependency.build.number' value='%dep.bt79.build.number%' />
        <param name='optional.parameters' value='-testFixture AddressService ' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers />
      <build-extensions>
        <extension id='BUILD_EXT_7' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
        <extension id='BUILD_EXT_6' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt79' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='poshsplice.yml' />
        </dependency>
        <dependency sourceBuildTypeId='bt6' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='*.zip' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt86' name='Artifacts'>
    <description />
    <settings ref='btTemplate8'>
      <parameters>
        <param name='build.number' value='%dep.bt324.build.number%' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt324' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt324' destinationPath='Artifacts' cleanDestination='true'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='InventoryManagement.CPE.zip' />
          <artifact sourcePath='poshsplice.yml' />
          <artifact sourcePath='Tests.Contracts.InventoryManagement.zip' />
        </dependency>
      </artifact-dependencies>
      <dependencies>
        <depend-on sourceBuildTypeId='bt85'>
          <options>
            <option name='take-started-build-with-same-revisions' value='true' />
            <option name='take-successful-builds-only' value='true' />
          </options>
        </depend-on>
        <depend-on sourceBuildTypeId='bt87'>
          <options>
            <option name='take-started-build-with-same-revisions' value='true' />
            <option name='take-successful-builds-only' value='true' />
          </options>
        </depend-on>
        <depend-on sourceBuildTypeId='bt90'>
          <options>
            <option name='take-started-build-with-same-revisions' value='true' />
            <option name='take-successful-builds-only' value='true' />
          </options>
        </depend-on>
        <depend-on sourceBuildTypeId='bt323' />
        <depend-on sourceBuildTypeId='bt483'>
          <options>
            <option name='take-started-build-with-same-revisions' value='true' />
            <option name='take-successful-builds-only' value='true' />
          </options>
        </depend-on>
      </dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt5' name='build'>
    <description />
    <settings ref='btTemplate3'>
      <parameters>
        <param name='build.number.format' value='1.1.0.%build.vcs.number.1%' />
        <param name='checkout.dir' value='cpebuild' />
      </parameters>
      <build-runners />
      <vcs-settings checkout-mode='INHERITED' labeling-type='INHERITED'>
        <vcs-entry-ref root-id='2' set-label='false' />
      </vcs-settings>
      <requirements />
      <build-triggers />
      <build-extensions>
        <extension id='BUILD_EXT_3' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='build/Unit-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt87' name='cdi / contract test'>
    <description />
    <settings ref='btTemplate9'>
      <parameters>
        <param name='app' value='cdi' />
        <param name='build.number' value='%dep.bt6.build.number%' />
        <param name='checkout.dir' value='cpe%app%deploycontractstest' />
        <param name='dependency.build.number' value='%dep.bt77.build.number%' />
        <param name='optional.parameters' value='-testFixture CustomerManagementService ' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers />
      <build-extensions>
        <extension id='BUILD_EXT_7' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
        <extension id='BUILD_EXT_6' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt77' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='poshsplice.yml' />
        </dependency>
        <dependency sourceBuildTypeId='bt6' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='*.zip' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt324' name='deploy / cpe / for contract test'>
    <description />
    <settings ref='btTemplate10'>
      <parameters>
        <param name='app' value='cpe' />
        <param name='build.number' value='%dep.bt6.build.number%' />
        <param name='dependency.build.number' value='%dep.bt6.build.number%' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt6' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt6' destinationPath='Artifacts' cleanDestination='true'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='*.zip' />
          <artifact sourcePath='poshsplice.yml' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt6' name='deploy / test'>
    <description />
    <settings ref='btTemplate4'>
      <parameters>
        <param name='app' value='cpe' />
        <param name='build.number' value='%dep.bt5.build.number%' />
        <param name='checkout.dir' value='cpedeploytest' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers>
        <build-trigger name='buildDependencyTrigger'>
          <parameters>
            <param name='afterSuccessfulBuildOnly' value='true' />
            <param name='dependsOn' value='bt5' />
          </parameters>
        </build-trigger>
      </build-triggers>
      <build-extensions>
        <extension id='BUILD_EXT_5' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
        <extension id='BUILD_EXT_4' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt5' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='*.zip' />
          <artifact sourcePath='poshsplice.yml' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt90' name='ffm / contract test'>
    <description />
    <settings ref='btTemplate9'>
      <parameters>
        <param name='app' value='ffm' />
        <param name='build.number' value='%dep.bt6.build.number%' />
        <param name='checkout.dir' value='cpe%app%deploycontractstest' />
        <param name='dependency.build.number' value='%dep.bt88.build.number%' />
        <param name='optional.parameters' value='-testFixture FieldForceManagementService' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers />
      <build-extensions>
        <extension id='BUILD_EXT_7' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
        <extension id='BUILD_EXT_6' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='%test.type%-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt88' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='poshsplice.yml' />
        </dependency>
        <dependency sourceBuildTypeId='bt6' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='*.zip' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
  <build-type id='bt171' name='Pin'>
    <description />
    <settings ref='btTemplate14'>
      <parameters>
        <param name='bt.id' value='bt86' />
        <param name='build.number' value='%system.Build.To.Pin%' />
        <param name='dependency.build.number' value='%dep.bt86.build.number%' />
      </parameters>
      <build-runners />
      <requirements />
      <build-triggers />
      <artifact-dependencies>
        <dependency sourceBuildTypeId='bt86' destinationPath='.' cleanDestination='false'>
          <revisionRule name='lastSuccessful' revision='latest.lastSuccessful' />
          <artifact sourcePath='poshsplice.yml' />
        </dependency>
      </artifact-dependencies>
      <cleanup />
    </settings>
  </build-type>
</project>

";
            #endregion
            // method under test
            var templates = new TemplateParser("pathToRootFolder").Parse(xml);
            Assert.IsNotNull(templates);
            Assert.AreEqual(3,templates.Count);
            var list = templates.Keys.ToList();
            Assert.AreEqual("btTemplate3",list[0]);
            Assert.AreEqual("btTemplate7",list[1]);
            Assert.AreEqual("btTemplate4",list[2]);
            string[] arr1= new string[2];
            arr1[0] = "sds";
            arr1[1] = "sds";
            string[] arr2 = new string[5];
            arr2[0] = "ggg";
            arr2[1] = "ggg";
            arr2[2] = "ggg";
            arr2[3] = "ggg";
            arr2[4] = "ggg";
            var array = arr1.Concat(arr2).ToArray();
        }

        [Test]
        public void ShouldGetAllTemplatesFromTeamCity()
        {

            var templateParser = new TemplateParser(Directory.GetCurrentDirectory() + "..\\..\\..\\TestData\\config");
            // method under test
            var allTemplates = templateParser.GetAllTemplates();
            Assert.IsNotNull(allTemplates);
        }
    }
}