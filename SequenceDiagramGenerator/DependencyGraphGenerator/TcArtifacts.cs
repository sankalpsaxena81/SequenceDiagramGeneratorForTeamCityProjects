namespace DependencyGraphGenerator
{
    public class TcArtifacts
    {
        public string BuildConfigurationId { get; private set; }
        public string ArtifactFile { get; private set; }

        public TcArtifacts(string buildConfigurationId, string artifactFile)
        {
            BuildConfigurationId = buildConfigurationId;
            ArtifactFile = artifactFile;
        }
    }
}