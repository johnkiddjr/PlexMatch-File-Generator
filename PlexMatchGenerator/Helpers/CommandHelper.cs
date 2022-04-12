using PlexMatchGenerator.Options;
using System.CommandLine;

namespace PlexMatchGenerator.Helpers
{
    public class CommandHelper
    {
        public static async Task<int> GenerateRootCommandAndExecuteHandler(string[] args, Func<GeneratorOptions, string[], Task<int>> handler)
        {
            var rootCommand = new RootCommand("PlexMatch File Generator - Generates .plexmatch files for existing library");

            var tokenAliases = new string[] { "--token", "-t", "/token", "/t" };
            var tokenDescription = "Plex Server Token more information on getting this at: https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/";
            var tokenOption = GenerateOption<string>(tokenAliases, tokenDescription, "Plex Server Token", "token", true);

            var urlAliases = new string[] { "--url", "-u", "/url", "/u" };
            var urlDescription = "URL to access your Plex server. Must include the http or https portion";
            var urlOption = GenerateOption<string>(urlAliases, urlDescription, "Plex Server URL", "url", true);

            var rootAliases = new string[] { "--root", "-r", "/root", "/r" };
            var rootDescription = "Sets the root path used to be different than what your Plex server returns, this option can be set more than once";
            var rootOption = GenerateOption<List<string>>(rootAliases, rootDescription, "Root Path Map", "root");

            var logAliases = new string[] { "--log", "-l", "/log", "/l" };
            var logDescription = "Outputs the log to file at the path specified, log file will be named plexmatch.log in the directory specified";
            var logOption = GenerateOption<string>(logAliases, logDescription, "Log Path", "log");

            rootCommand.AddOption(tokenOption);
            rootCommand.AddOption(urlOption);
            rootCommand.AddOption(rootOption);
            rootCommand.AddOption(logOption);

            rootCommand.SetHandler(
                async (string token, string url, List<string> rootPaths, string log) =>
                {
                    var generatorOptions = ArgumentHelper.ProcessCommandLineResults(token, url, rootPaths, log);
                    await handler(generatorOptions, args);
                },
                tokenOption,
                urlOption,
                rootOption,
                logOption);

            return await rootCommand.InvokeAsync(args);
        }

        private static Option<T> GenerateOption<T>(string[] aliases, string description, string helpName, string name, bool required = false)
        {
            var option = new Option<T>(aliases, description);
            option.ArgumentHelpName = helpName;
            option.Name = name;
            option.IsRequired = required;

            return option;
        }
    }
}
