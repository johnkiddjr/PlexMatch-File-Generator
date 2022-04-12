using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Options;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;

namespace PlexMatchGenerator.Helpers
{
    public class ArgumentHelper
    {
        public static GeneratorOptions ProcessCommandLineResults(string plexToken, string plexUrl, List<string> rootPaths, string logPath)
        {
            return new GeneratorOptions
            {
                LogPath = logPath,
                PlexServerUrl = plexUrl,
                PlexServerToken = plexToken,
                RootPaths = GenerateRootPaths(rootPaths)
            };
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

            return plexUrl.StartsWith(HttpConstants.UnsecureProtocol) || plexUrl.StartsWith(HttpConstants.SecureProtocol);
        }

        // This stub exists for potential future expansion only
        public static bool ValidatePlexToken(string plexToken) =>
            !string.IsNullOrEmpty(plexToken);
    }
}
