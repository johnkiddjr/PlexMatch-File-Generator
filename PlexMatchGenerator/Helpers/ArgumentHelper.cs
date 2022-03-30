using PlexMatchGenerator.Options;

namespace PlexMatchGenerator.Helpers
{
    public class ArgumentHelper
    {
        public static GeneratorOptions ProcessCommandLineArguments(List<string> args)
        {
            string plexUrl = string.Empty;
            string plexToken = string.Empty;
            string plexPath = string.Empty;
            string hostPath = string.Empty;
            string logPath = string.Empty;

            if (args.Count > 1)
            {
                var urlArgument = args.Where(arg => arg == "--url" || arg == "-u").FirstOrDefault();
                var tokenArgument = args.Where(arg => arg == "--token" || arg == "-t").FirstOrDefault();
                var pathArgument = args.Where(arg => arg == "--root" || arg == "-r").FirstOrDefault();
                var logArgument = args.Where(arg => arg == "--log" || arg == "-l").FirstOrDefault();

                if (urlArgument != null)
                {
                    plexUrl = args[args.IndexOf(urlArgument) + 1];
                }

                if (tokenArgument != null)
                {
                    plexToken = args[args.IndexOf(tokenArgument) + 1];
                }

                if (pathArgument != null)
                {
                    var rootPath = args[args.IndexOf(pathArgument) + 1].Split(':');

                    if (rootPath.Length == 2)
                    {
                        hostPath = rootPath[0];
                        plexPath = rootPath[1];
                    }
                }

                if (logArgument != null)
                {
                    logPath = args[args.IndexOf(logArgument) + 1];

                    if (!logPath.EndsWith("/"))
                    {
                        logPath += "/";
                    }
                }
            }

            return new GeneratorOptions
            {
                PlexServerUrl = plexUrl,
                PlexServerToken = plexToken,
                LogPath = logPath,
                HostRootPath = hostPath,
                PlexRootPath = plexPath
            };
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
