namespace PlexMatchGenerator.Options
{
    public class GeneratorOptions
    {
        public string PlexServerUrl { get; set; }
        public string PlexServerToken { get; set; }        
        public string LogPath { get; set; }

        public Dictionary<string, string> DirectoriesMapping { get; set; }
    }
}
