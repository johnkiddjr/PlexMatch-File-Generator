using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Options;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;

namespace PlexMatchGenerator.Helpers
{
    public class ArgumentHelper
    {
        public static GeneratorOptions ProcessCommandLineParseResultsToGeneratorOptions(RootCommand rootCommand, CommandResult result)
        {
            var options = new GeneratorOptions();

            foreach (var option in rootCommand.Options)
            {
                switch (option.Name)
                {
                    case "log":
                        options.LogPath = result.GetValueForOption((Option<string>)option);
                        break;
                    case "root":
                        options.RootPaths = GenerateRootPaths(result.GetValueForOption((Option<List<string>>)option));
                        break;
                    case "url":
                        options.PlexServerUrl = result.GetValueForOption((Option<string>)option);
                        break;
                    case "token":
                        options.PlexServerToken = result.GetValueForOption((Option<string>)option);
                        break;
                    default:
                        break;
                }
            }

            return options;
        }

        private static List<RootPathOptions> GenerateRootPaths(List<string> rootMaps)
        {
            if (!rootMaps.Any())
            {
                return null;
            }

            var rootPathMaps = new List<RootPathOptions>();
            
            foreach (var rootMap in rootMaps)
            {
                var pathMatches = Regex.Matches(rootMap, RegexConstants.RootPathMatchPattern);

                if (pathMatches.Count > 1)
                {
                    var newRootPath = new RootPathOptions();

                    foreach (Match match in pathMatches)
                    {
                        if (string.IsNullOrWhiteSpace(match.Value))
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(newRootPath.HostRootPath))
                        {
                            newRootPath.HostRootPath = match.Value;
                        }
                        else
                        {
                            newRootPath.PlexRootPath = match.Value;
                            break;
                        }
                    }

                    rootPathMaps.Add(newRootPath);
                }
            }

            return rootPathMaps;
        }

        public static bool ValidatePlexUrl(string plexUrl)
        {
            if (!plexUrl.EndsWith("/"))
            {
                plexUrl += "/";
            }

            return plexUrl.StartsWith("http://") || plexUrl.StartsWith("https://");
        }

        // This stub exists for potential future expansion only
        public static bool ValidatePlexToken(string plexToken) =>
            !string.IsNullOrEmpty(plexToken);
    }
}
