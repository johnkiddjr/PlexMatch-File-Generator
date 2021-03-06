using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Options;
using System.CommandLine;

namespace PlexMatchGenerator.Helpers
{
    public class CommandHelper
    {
        public static async Task<int> GenerateRootCommandAndExecuteHandler(string[] args, Func<GeneratorOptions, string[], Task<int>> handler)
        {
            var rootCommand = new RootCommand(CommandConstants.RootDescription);

            var tokenAliases = new string[] 
            {
                CommandConstants.TokenCommandUnixLong,
                CommandConstants.TokenCommandUnixShort,
                CommandConstants.TokenCommandWindowsLong,
                CommandConstants.TokenCommandWindowsShort
            };

            var tokenOption = GenerateOption<string>(
                    tokenAliases,
                    CommandConstants.TokenCommandDescription,
                    CommandConstants.TokenCommandHelpName,
                    CommandConstants.TokenCommandName,
                    true);

            var urlAliases = new string[]
            {
                CommandConstants.UrlCommandUnixLong,
                CommandConstants.UrlCommandUnixShort,
                CommandConstants.UrlCommandWindowsLong,
                CommandConstants.UrlCommandWindowsShort
            };

            var urlOption = GenerateOption<string>(
                urlAliases,
                CommandConstants.UrlCommandDescription,
                CommandConstants.UrlCommandHelpName,
                CommandConstants.UrlCommandName,
                true);

            var rootAliases = new string[]
            {
                CommandConstants.RootPathCommandUnixLong,
                CommandConstants.RootPathCommandUnixShort,
                CommandConstants.RootPathCommandWindowsLong,
                CommandConstants.RootPathCommandWindowsShort
            };

            var rootOption = GenerateOption<List<string>>(
                rootAliases,
                CommandConstants.RootPathCommandDescription,
                CommandConstants.RootPathCommandHelpName,
                CommandConstants.RootPathCommandName);

            var logAliases = new string[]
            {
                CommandConstants.LogPathCommandUnixLong,
                CommandConstants.LogPathCommandUnixShort,
                CommandConstants.LogPathCommandWindowsLong,
                CommandConstants.LogPathCommandWindowsShort
            };

            var logOption = GenerateOption<string>(
                logAliases,
                CommandConstants.LogPathCommandDescription,
                CommandConstants.LogPathCommandHelpName,
                CommandConstants.LogPathCommandName);

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
