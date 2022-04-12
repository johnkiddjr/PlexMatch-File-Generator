//using CommandLine;

namespace PlexMatchGenerator.Options
{
    public class CommandLineOptions
    {
       // [Option('t', "token", Required = true, HelpText = "Plex Server Token more information on getting this at: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/")]
        public string PlexServerToken { get; set; }
     //   [Option('u', "url", Required = true, HelpText = "URL to access your Plex server. Must include the http or https portion.")]
        public string PlexServerUrl { get; set; }
       // [Option('r', "root", HelpText = "Sets the root path used to be different than what your Plex server returns")]
        public IEnumerable<string> RootPaths { get; set; }
     //   [Option('l', "log", HelpText = "Outputs the log to file at the path specified, log file will be named plexmatch.log in the directory specified")]
        public string LogPath { get; set; }
    }
}
