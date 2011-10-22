using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DependencyGraphGenerator;
using NUnit.Framework;

namespace DependencyGraphGeneratorTest
{
    [TestFixture]
    public class TemplateMergerTest
    {
        [Test]
        public void ShouldMergeTemplateWithConfig()
        {
            #region template
            string template1 = @"<template id='btTemplate3' name='build'>
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
  </template>";
#endregion
            #region configXml
            string configXml = @"<?xml version='1.0' encoding='UTF-8'?>
<!DOCTYPE project SYSTEM '../project-config.dtd'>

<project id='project9'>
  <parameters />
 <build-type id='bt29' name='Staging - build'>
    <description />
    <settings ref='btTemplate3'>
      <parameters>
        <param name='build.number.format' value='1.1.0.%build.vcs.number.1%' />
        <param name='checkout.dir' value='aisstagingbuild' />
      </parameters>
      <build-runners />
      <vcs-settings checkout-mode='INHERITED' labeling-type='INHERITED'>
        <vcs-entry-ref root-id='21' set-label='false' />
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
        <extension id='BUILD_EXT_2' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='build/Unit-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <cleanup />
    </settings>
  </build-type>
  </project>";
            #endregion

            #region mergedXml
            string mergedXml=@"<project id='project9'>
  <parameters />
  <build-type id='bt29' name='Staging - build'>
    <description />
    <settings>
      <options>
        <option name='allowExternalStatus' value='true' />
        <option name='buildNumberPattern' value='%build.number.format%' />
        <option name='maximumNumberOfBuilds' value='1' />
      </options>
      <parameters>
        <param name='build.number.format' value='1.0.0.{0}' />
        <param name='checkout.dir' value='' />
        <param name='build.number.format' value='1.1.0.%build.vcs.number.1%' />
        <param name='checkout.dir' value='aisstagingbuild' />
      </parameters>
      <build-runners>
        <runner id='RUNNER_8' name='' type='simpleRunner'>
          <parameters>
            <param name='command.executable' value='build.bat' />
          </parameters>
        </runner>
      </build-runners>
      <vcs-settings checkout-mode='ON_AGENT' checkout-dir='%checkout.dir%' labeling-type='NONE' labeling-pattern='build-%system.build.number%'>
        <vcs-entry-ref root-id='21' set-label='false' />
      </vcs-settings>
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
        <extension id='BUILD_EXT_3' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='build/Unit-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
        <extension id='BUILD_EXT_2' type='xml-report-plugin'>
          <parameters>
            <param name='xmlReportParsing.reportDirs' value='build/Unit-Test-Reports.xml' />
            <param name='xmlReportParsing.reportType' value='nunit' />
          </parameters>
        </extension>
      </build-extensions>
      <artifact-publishing paths='build/*.zip&#xA;build/*.xml&#xA;build/*.html&#xA;build/package/poshsplice.yml' />
      <cleanup />
    </settings>
  </build-type>
</project>";
            #endregion
            // given a dictionary of templates merge them with files
            var dictionary = new Dictionary<string, XElement>();
            dictionary.Add("btTemplate3",XElement.Parse(template1));
//            string rootFolder = Directory.GetCurrentDirectory() + "..\\..\\..\\TestData\\config";
            var templateMerger = new TemplateMerger(dictionary,"rootFolder");
            
//            var mergedXelment = templateMerger.MergeTemplatesWithConfig(XElement.Parse(configXml));
//            Assert.IsTrue(mergedXelment.ToString().Equals(XElement.Parse(mergedXml).ToString()));

            var mergeAllProjectConfigs = templateMerger.MergeAllProjectConfigs();
            var tcObjectBuilder = new TcObjectBuilder();
            var tcProjects = tcObjectBuilder.Parse(mergeAllProjectConfigs);
        }
    }
}