using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Options;
using System.Text.RegularExpressions;

namespace PlexMatchGenerator.Helpers
{
    public class ArgumentHelper
    {
        public static GeneratorOptions ProcessCommandLineResults(
            string plexToken, 
            string plexUrl, 
            List<string> rootPaths, 
            string logPath, 
            bool noOverwrite, 
            int pageSize, 
            List<string> libraries, 
            List<string> shows,
            bool seasonProcessing)
        {
            //ensure we end the path with a slash
            if (logPath != null && !logPath.EndsWith("\\") && !logPath.EndsWith('/'))
            {
                //make sure we use the correct slash if we need to use it
                bool useBackslash = logPath.Count(x => x.Equals('\\')) > logPath.Count(x => x.Equals('/'));

                logPath += useBackslash ? "\\" : "/";
            }

            return new GeneratorOptions
            {
                LogPath = logPath,
                PlexServerUrl = plexUrl,
                PlexServerToken = plexToken,
                RootPaths = GenerateRootPaths(rootPaths),
                NoOverwrite = noOverwrite,
                ItemsPerPage = pageSize == 0 ? 20 : pageSize,
                LibraryNames = libraries,
                ShowNames = shows,
                EnablePerSeasonProcessing = seasonProcessing
            };
        }

        private static List<RootPathOptions> GenerateRootPaths(List<string> rootMaps)
        {
            if (!rootMaps.Any())
            {
                return new List<RootPathOptions>();
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
