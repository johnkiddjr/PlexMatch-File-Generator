using PlexMatchGenerator.Options;

namespace PlexMatchGenerator.Helpers
{
    public class ArgumentHelper
    {
        public static GeneratorOptions ProcessCommandLineArguments(List<string> args)
        {
            var options = new GeneratorOptions
            {
                PlexServerUrl = string.Empty,
                PlexServerToken = string.Empty,
                LogPath = string.Empty,
                DirectoriesMapping = new()
            };

            var reader = new Queue<string>(args);

            while (reader.Count > 0)
            {
                switch (reader.Dequeue())
                {
                    case "--url" or "-u":
                        if (reader.Count > 0)
                        {
                            options.PlexServerUrl = reader.Dequeue();
                        }
                        break;

                    case "--token" or "-t":
                        if (reader.Count > 0)
                        {
                            options.PlexServerToken = reader.Dequeue();
                        }
                        break;

                    case "--root" or "-r":
                        TryReadOneRootArgs(); // ensure read one
                        while (reader.Count > 0 && !reader.Peek().StartsWith("-"))
                        {
                            TryReadOneRootArgs(); // read more
                        }
                        break;

                    case "--log" or "-l":
                        if (reader.Count > 0)
                        {
                            var logPath = reader.Dequeue();
                            if (!logPath.EndsWith("/"))
                            {
                                logPath += "/";
                            }
                            options.LogPath = logPath;
                        }
                        break;
                        
                    default:
                        break;
                }
            }

            return options;

            void TryReadOneRootArgs()
            {
                if (reader.Count > 0)
                {
                    var rootPathArgs = reader.Dequeue().Split("::");

                    if (rootPathArgs.Length == 2)
                    {
                        var hostPath = rootPathArgs[0];
                        var plexPath = rootPathArgs[1];
                        if (!string.IsNullOrEmpty(hostPath) && !string.IsNullOrEmpty(plexPath))
                        {
                            options.DirectoriesMapping[plexPath] = hostPath;
                        }
                    }
                }
            }
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
