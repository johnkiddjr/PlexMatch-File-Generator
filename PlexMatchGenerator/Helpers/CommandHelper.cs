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
                CommandConstants.TokenCommandWindowsShort,
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

            var noOverwriteAliases = new string[]
            {
                CommandConstants.NoOverwriteCommandUnixLong,
                CommandConstants.NoOverwriteCommandUnixShort,
                CommandConstants.NoOverwriteCommandWindowsLong,
                CommandConstants.NoOverwriteCommandWindowsShort
            };

            var noOverwriteOption = GenerateOption<bool>(
                noOverwriteAliases,
                CommandConstants.NoOverwriteCommandDescription,
                CommandConstants.NoOverwriteCommandHelpName,
                CommandConstants.NoOverwriteCommandName);

            var pageSizeAliases = new string[]
            {
                CommandConstants.PageSizeCommandUnixLong,
                CommandConstants.PageSizeCommandUnixShort,
                CommandConstants.PageSizeCommandWindowsLong,
                CommandConstants.PageSizeCommandWindowsShort
            };

            var pageSizeOption = GenerateOption<int>(
                pageSizeAliases,
                CommandConstants.PageSizeCommandDescription,
                CommandConstants.PageSizeCommandHelpName,
                CommandConstants.PageSizeCommandName);

            var libraryAliases = new string[]
            {
                CommandConstants.LibraryCommandUnixLong,
                CommandConstants.LibraryCommandUnixShort,
                CommandConstants.LibraryCommandWindowsLong,
                CommandConstants.LibraryCommandWindowsShort
            };

            var libraryOption = GenerateOption<List<string>>(
                libraryAliases,
                CommandConstants.LibraryCommandDescription,
                CommandConstants.LibraryCommandHelpName,
                CommandConstants.LibraryCommandName);

            var showAliases = new string[]
            {
                CommandConstants.ShowCommandUnixLong,
                CommandConstants.ShowCommandUnixShort,
                CommandConstants.ShowCommandWindowsLong,
                CommandConstants.ShowCommandWindowsShort
            };

            var showOption = GenerateOption<List<string>>(
                showAliases,
                CommandConstants.ShowCommandDescription,
                CommandConstants.ShowCommandHelpName,
                CommandConstants.ShowCommandName);

            var perSeasonProcessingAliases = new string[]
            {
                CommandConstants.PerSeasonProcessingCommandUnixLong,
                CommandConstants.PerSeasonProcessingCommandUnixShort,
                CommandConstants.PerSeasonProcessingCommandWindowsLong,
                CommandConstants.PerSeasonProcessingCommandWindowsShort
            };

            var perSeasonProcessingOption = GenerateOption<bool>(
                perSeasonProcessingAliases,
                CommandConstants.PerSeasonProcessingCommandDescription,
                CommandConstants.PerSeasonProcessingCommandHelpName,
                CommandConstants.PerSeasonProcessingCommandName);

            rootCommand.AddOption(tokenOption);
            rootCommand.AddOption(urlOption);
            rootCommand.AddOption(rootOption);
            rootCommand.AddOption(logOption);
            rootCommand.AddOption(pageSizeOption);
            rootCommand.AddOption(noOverwriteOption);
            rootCommand.AddOption(libraryOption);
            rootCommand.AddOption(showOption);
            rootCommand.AddOption(perSeasonProcessingOption);

            rootCommand.SetHandler(
                async (context) =>
                {
                    var token = context.ParseResult.GetValueForOption(tokenOption);
                    var url = context.ParseResult.GetValueForOption(urlOption);
                    var rootPaths = context.ParseResult.GetValueForOption(rootOption);
                    var log = context.ParseResult.GetValueForOption(logOption);
                    var pageSize = context.ParseResult.GetValueForOption(pageSizeOption);
                    var overwrite = context.ParseResult.GetValueForOption(noOverwriteOption);
                    var libraries = context.ParseResult.GetValueForOption(libraryOption);
                    var shows = context.ParseResult.GetValueForOption(showOption);
                    var seasonProcessing = context.ParseResult.GetValueForOption(perSeasonProcessingOption);

                    var generatorOptions = ArgumentHelper.ProcessCommandLineResults(
                        token,
                        url,
                        rootPaths,
                        log,
                        overwrite,
                        pageSize,
                        libraries,
                        shows,
                        seasonProcessing);

                    await handler(generatorOptions, args);
                });

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
