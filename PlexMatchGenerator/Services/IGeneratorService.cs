using Microsoft.Extensions.Logging;
using PlexMatchGenerator.Abstractions;
using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.Options;
using PlexMatchGenerator.RestModels;
using RestSharp;

namespace PlexMatchGenerator.Services
{
    public interface IGeneratorService
    {
        public Task<int> Run(GeneratorOptions options);
    }

    public class GeneratorService : IGeneratorService
    {
        private readonly ILogger logger;

        public GeneratorService(ILogger<GeneratorService> logger)
        {
            this.logger = logger;
        }

        public async Task<int> Run(GeneratorOptions options)
        {
            if (string.IsNullOrEmpty(options.PlexServerUrl))
            {
                Console.WriteLine("Please enter your Plex Token: ");
                options.PlexServerToken = Console.ReadLine();
            }

            if (!ArgumentHelper.ValidatePlexUrl(options.PlexServerUrl))
            {
                logger.LogCritical("Plex Server URL must be set, and must start with either http:// or https://");
                return 1;
            }

            if (string.IsNullOrEmpty(options.PlexServerToken))
            {
                Console.WriteLine("Please enter your Plex Server URL: ");
                options.PlexServerUrl = Console.ReadLine();
            }

            if (!ArgumentHelper.ValidatePlexToken(options.PlexServerToken))
            {
                logger.LogCritical("Plex Server Token must be set");
                return 1;
            }

            // connect to plex and get a list of libraries
            try
            {
                var client = RestClientHelper.GenerateClient(options.PlexServerUrl, options.PlexServerToken);

                var libraryRoot = await RestClientHelper.CreateAndGetRestResponse<LibraryRoot>(client, PlexApiUrlConstants.LibrarySectionsRequestUrl, Method.Get);

                var libraries = libraryRoot?.LibraryContainer?.Libraries;

                if (libraries is null)
                {
                    logger.LogError("No data or malformed data received from server when querying for libraries");
                    return 1;
                }

                // step through the libraries and get a list of the items and their folders
                foreach (var library in libraries)
                {
                    var itemRoot = await RestClientHelper.CreateAndGetRestResponse<MediaItemRoot>(client, $"{PlexApiUrlConstants.LibrarySectionsRequestUrl}/{library.LibraryId}/{PlexApiUrlConstants.SearchAll}", Method.Get);

                    var items = itemRoot?.MediaItemContainer?.MediaItems;

                    if (items is null)
                    {
                        logger.LogError("Library {libraryName} of type {libraryType} with ID {libraryID} returned no items", library.LibraryName, library.LibraryType, library.LibraryId);
                        continue;
                    }

                    // step through each item in the library and get it's tvdbid and drop a .plexmatch file in it's root
                    foreach (var item in items)
                    {
                        var locationInfoRoot = await RestClientHelper.CreateAndGetRestResponse<MediaItemInfoRoot>(client, $"{PlexApiUrlConstants.MetaDataRequestUrl}/{item.MediaItemId}", Method.Get);

                        var locationInfos = locationInfoRoot?.MediaItemInfoContainer?.MediaItemInfos;

                        if (locationInfos is null)
                        {
                            logger.LogError("Item with title {itemTitle} and ID {itemId} returned no location information", item.MediaItemTitle, item.MediaItemId);
                            continue;
                        }

                        foreach (var locationInfo in locationInfos)
                        {
                            List<IMediaPath> possibleMediaLocations = new List<IMediaPath>();

                            if (library.LibraryType == "movie" && locationInfo.MediaInfos != null)
                            {
                                possibleMediaLocations = locationInfo.MediaInfos.SelectMany(mi => mi.MediaParts).Select(mp => (IMediaPath)mp).ToList();

                                possibleMediaLocations.ForEach(pml =>
                                {
                                    var lastForwardSlash = pml.MediaItemPath.LastIndexOf("/");
                                    var lastBackwardSlash = pml.MediaItemPath.LastIndexOf(@"\");

                                    pml.MediaItemPath = pml.MediaItemPath.Substring(0, (lastBackwardSlash > lastForwardSlash) ? lastBackwardSlash : lastForwardSlash);
                                });
                            }
                            else if ((library.LibraryType == "show" || library.LibraryType == "artist") && locationInfo.MediaItemLocations != null)
                            {
                                possibleMediaLocations = locationInfo.MediaItemLocations.Select(mil => (IMediaPath)mil).ToList();
                            }
                            else
                            {
                                logger.LogWarning($"No media location found for: {item.MediaItemTitle}");
                                continue;
                            }

                            foreach (var location in possibleMediaLocations)
                            {
                                var mediaPath = location.MediaItemPath;

                                //if (!string.IsNullOrEmpty(options.PlexRootPath) && !string.IsNullOrEmpty(options.HostRootPath) && mediaPath.StartsWith(options.PlexRootPath))
                                //{
                                //    mediaPath = mediaPath.Replace(options.PlexRootPath, options.HostRootPath);
                                //}

                                if (Directory.Exists(mediaPath))
                                {
                                    using StreamWriter sw = new StreamWriter($"{mediaPath}/{MediaConstants.PlexMatchFileName}", false);
                                    sw.WriteLine($"{MediaConstants.PlexMatchTitleHeader}{item.MediaItemTitle}");
                                    sw.WriteLine($"{MediaConstants.PlexMatchYearHeader}{item.MediaItemReleaseYear}");
                                    sw.WriteLine($"{MediaConstants.PlexMatchGuidHeader}{item.MediaItemPlexMatchGuid}");

                                    logger.LogInformation($"{MessageConstants.PlexMatchWritten} {item.MediaItemTitle}");
                                }
                                else
                                {
                                    logger.LogError($"{MessageConstants.FolderMissingOrInvalid} {mediaPath}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("An unhandeled exception occurred details below:");
                logger.LogError("Exception Type: {exceptionType}", ex.GetType().ToString());
                logger.LogError("Exception Message: {exceptionMessage}", ex.Message);
                if (ex.InnerException != null)
                {
                    logger.LogError("Inner Exception Type: {innerType}", ex.InnerException.GetType().ToString());
                    logger.LogError("Inner Exception Message: {innerMessage}", ex.InnerException.Message);
                }
                logger.LogError("Exception Source: {exceptionSource}", ex.Source);
                logger.LogError("Exception Stack Trace: {stackTrace}", ex.StackTrace);

                return 1;
            }

            logger.LogInformation(MessageConstants.CompletedMessage);
            return 0;
        }
    }
}
