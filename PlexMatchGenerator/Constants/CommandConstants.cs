namespace PlexMatchGenerator.Constants
{
    public class CommandConstants
    {
        public const string RootDescription = "PlexMatch File Generator - Generates .plexmatch files for existing library";

        public const string TokenCommandDescription = "Plex Server Token more information on getting this at: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/";
        public const string TokenCommandHelpName = "Plex Server Token";
        public const string TokenCommandName = "token";
        public const string TokenCommandShortName = "t";
        public const string TokenCommandUnixLong = $"--{TokenCommandName}";
        public const string TokenCommandUnixShort = $"-{TokenCommandShortName}";
        public const string TokenCommandWindowsLong = $"/{TokenCommandName}";
        public const string TokenCommandWindowsShort = $"/{TokenCommandShortName}";

        public const string UrlCommandDescription = "URL to access your Plex server. Must include the http or https portion";
        public const string UrlCommandHelpName = "Plex Server URL";
        public const string UrlCommandName = "url";
        public const string UrlCommandShortName = "u";
        public const string UrlCommandUnixLong = $"--{UrlCommandName}";
        public const string UrlCommandUnixShort = $"-{UrlCommandShortName}";
        public const string UrlCommandWindowsLong = $"/{UrlCommandName}";
        public const string UrlCommandWindowsShort = $"/{UrlCommandShortName}";

        public const string RootPathCommandDescription = "Sets the root path used to be different than what your Plex server returns, this option can be set more than once";
        public const string RootPathCommandHelpName = "Root Path Map";
        public const string RootPathCommandName = "root";
        public const string RootPathCommandShortName = "r";
        public const string RootPathCommandUnixLong = $"--{RootPathCommandName}";
        public const string RootPathCommandUnixShort = $"-{RootPathCommandShortName}";
        public const string RootPathCommandWindowsLong = $"/{RootPathCommandName}";
        public const string RootPathCommandWindowsShort = $"/{RootPathCommandShortName}";

        public const string LogPathCommandDescription = "Outputs the log to file at the path specified, log file will be named plexmatch.log in the directory specified";
        public const string LogPathCommandHelpName = "Log Path";
        public const string LogPathCommandName = "log";
        public const string LogPathCommandShortName = "l";
        public const string LogPathCommandUnixLong = $"--{LogPathCommandName}";
        public const string LogPathCommandUnixShort = $"-{LogPathCommandShortName}";
        public const string LogPathCommandWindowsLong = $"/{LogPathCommandName}";
        public const string LogPathCommandWindowsShort = $"/{LogPathCommandShortName}";
    }
}
