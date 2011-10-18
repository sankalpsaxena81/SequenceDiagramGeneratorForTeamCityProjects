namespace DependencyGraphGenerator
{
    public interface ITcObjectBuilder
    {
        TcProjects Parse(string rootfolder);
    }
}