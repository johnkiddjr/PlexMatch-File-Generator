namespace PlexMatchGenerator.Helpers
{
    public class ArgumentHelper
    {
        public static (string PlexServerUrl, string PlexToken) ProcessCommandLineArguments(List<string> args)
        {
            string plexUrl = string.Empty;
            string plexToken = string.Empty;

            if (args.Count > 1)
            {
                var urlArgument = args.Where(arg => arg == "--url" || arg == "-u").FirstOrDefault();
                var tokenArgument = args.Where(arg => arg == "--token" || arg == "-t").FirstOrDefault();

                if (urlArgument != null)
                {
                    plexUrl = args[args.IndexOf(urlArgument) + 1];
                }

                if (tokenArgument != null)
                {
                    plexToken = args[args.IndexOf(tokenArgument) + 1];
                }
            }

            return (plexUrl, plexToken);
        }

        public static bool CheckAndGetIfPlexTokenBlank(ref string plexToken)
        {
            if (string.IsNullOrEmpty(plexToken))
            {
                Console.WriteLine("Please enter your Plex Token: ");
                plexToken = Console.ReadLine();
            }

            return ValidatePlexToken(plexToken);
        }

        public static bool CheckAndGetIfPlexUrlBlank(ref string plexUrl)
        {
            if (string.IsNullOrEmpty(plexUrl))
            {
                Console.WriteLine("Please enter your Plex Token: ");
                plexUrl = Console.ReadLine();
            }

            return ValidatePlexUrl(plexUrl);
        }

        private static bool ValidatePlexUrl(string plexUrl)
        {
            if (!plexUrl.EndsWith("/"))
            {
                plexUrl += "/";
            }

            return plexUrl.StartsWith("http://") || plexUrl.StartsWith("https://");
        }

        // This stub exists for potential future expansion only
        private static bool ValidatePlexToken(string plexToken) =>
            string.IsNullOrEmpty(plexToken);
    }
}
