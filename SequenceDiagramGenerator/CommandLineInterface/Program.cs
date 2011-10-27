using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DependencyGraphGenerator;
using WebSequenceDiagramsGenerator;

namespace CommandLineInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args != null && args.Length == 2)
            {
                var rootFolder = args[0];
                var saveDiagramsAtPath = args[1];
                var diagramsGenerator = new DiagramsGenerator(new TcObjectBuilder(), rootFolder);
                var hasSaved = diagramsGenerator.GenerateDiagramsAndSaveAsFilesInDirectory(
                    saveDiagramsAtPath);
                var message = hasSaved
                            ? string.Format("Diagrams generated Successfuly at location{0}", saveDiagramsAtPath)
                            : "Encountered an Error generating the diagrams";

                Console.WriteLine(message);
            }
            else
            {
                Console.WriteLine("Insifficient arguments. Usage: ");
                Console.WriteLine(
                    "WebSequenceDiagramGeneratorCmd \"PathToTeamCityConfigsRootFolder\" \"PathToDirectoryWhereDiagramsShouldBeStored\" ");
            }
            Console.Read();
        }
    }
}
