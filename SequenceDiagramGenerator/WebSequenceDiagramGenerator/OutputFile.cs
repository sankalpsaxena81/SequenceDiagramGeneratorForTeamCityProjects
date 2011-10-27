using System;
using System.IO;
using System.Linq;
using DependencyGraphGenerator;

namespace WebSequenceDiagramsGenerator
{
    public class OutputFile
    {
        public TcProject ProjName { get; private set; }
        public string BuildConfName { get;private  set; }
        public string FinalString { get; private set; }



        public OutputFile(TcProject projName, string buildConfName, string finalString)
        {
            ProjName = projName;
            BuildConfName = buildConfName;
            FinalString = finalString;
        }

        private string[] CreateHtmlFileWithSequenceDiagram()
        {

            var stringToBeWritten = GetStringToBeWritten(FinalString);
            string[] lines = new string[1];
            lines[0] = CreateFirstLine();
            lines = lines.Concat(CreateSequenceDiagramLines(stringToBeWritten)).ToArray();
            lines = lines.Concat(new string[1] { CreateLastLine() }).ToArray();
            return lines;
        }
        private string[] GetStringToBeWritten(string s)
        {
            return s.Split('#');
        }
        private string CreateFirstLine()
        {
            return "<div class=wsd wsd_style=\"modern-blue\" ><pre>";
        }
        private string[] CreateSequenceDiagramLines(string[] stringToBeWritten)
        {
            string[] lines = new string[stringToBeWritten.Length];
            int i = 0;
            foreach (var str in stringToBeWritten)
                lines[i++] = str;
            return lines;
        }

        private string CreateLastLine()
        {
            return "</pre></div><script type=\"text/javascript\" src=\"http://www.websequencediagrams.com/service.js\"></script>";
        }

        private bool WriteContentToFile(string[] content, string path)
        {
            bool hasSavesAllFiles = true;
            try
            {
                File.WriteAllLines(string.Format("{0}\\{1}-{2}.html", path,ProjName.ProjectName, BuildConfName), content);
            }
            catch (Exception)
            {

                hasSavesAllFiles = false;
            }
            return hasSavesAllFiles;
        }


        public bool SaveFile(string path)
        {
            var content = CreateHtmlFileWithSequenceDiagram();
            return WriteContentToFile(content, path);
        }
    }
}