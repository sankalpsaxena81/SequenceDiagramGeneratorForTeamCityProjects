namespace DependencyGraphGenerator
{
    public interface ITcObjectBuilder
    {
        TcProjects BuildObjectGraph(string rootFolder);
    }
}