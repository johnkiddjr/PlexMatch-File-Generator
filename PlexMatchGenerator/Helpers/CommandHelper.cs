using System.CommandLine;

namespace PlexMatchGenerator.Helpers
{
    public class CommandHelper
    {
        public static RootCommand GenerateRootCommandHandler()
        {
            var rootCommand = new RootCommand();

            var tokenAliases = new string[] { "--token", "-t", "/token", "/t" };
            var tokenDescription = "Plex Server Token more information on getting this at: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/";
            var tokenOption = GenerateOption<string>(tokenAliases, tokenDescription, "Plex Server Token", "token");

            var urlAliases = new string[] { "--url", "-u", "/url", "/u" };
            var urlDescription = "URL to access your Plex server. Must include the http or https portion.";
            var urlOption = GenerateOption<string>(urlAliases, urlDescription, "Plex Server URL", "url");

            var rootAliases = new string[] { "--root", "-r", "/root", "/r" };
            var rootDescription = "Sets the root path used to be different than what your Plex server returns";
            var rootOption = GenerateOption<List<string>>(rootAliases, rootDescription, "Root Path Maps", "root");

            var logAliases = new string[] { "--log", "-l", "/log", "/l" };
            var logDescription = "Outputs the log to file at the path specified, log file will be named plexmatch.log in the directory specified";
            var logOption = GenerateOption<string>(logAliases, logDescription, "Log Path", "log");

            rootCommand.Add(tokenOption);
            rootCommand.Add(urlOption);
            rootCommand.Add(rootOption);
            rootCommand.Add(logOption);

            return rootCommand;
        }

        private static Option<T> GenerateOption<T>(string[] aliases, string description, string helpName, string name)
        {
            var option = new Option<T>(aliases, description);
            option.ArgumentHelpName = helpName;
            option.Name = name;

            return option;
        }
    }
}
