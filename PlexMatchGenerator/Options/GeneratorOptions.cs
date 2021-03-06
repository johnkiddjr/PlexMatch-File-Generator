namespace PlexMatchGenerator.Options
{
    public class GeneratorOptions
    {
        public string PlexServerUrl { get; set; }
        public string PlexServerToken { get; set; }
        public IEnumerable<RootPathOptions> RootPaths { get; set; }
        public string LogPath { get; set; }
    }
}
