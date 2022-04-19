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
            if (string.IsNullOrEmpty(options.PlexServerToken))
            {
                Console.WriteLine(MessageConstants.EnterPlexToken);
                options.PlexServerToken = Console.ReadLine();
            }

            if (!ArgumentHelper.ValidatePlexToken(options.PlexServerToken))
            {
                logger.LogCritical(MessageConstants.TokenInvalid);
                return 1;
            }

            if (string.IsNullOrEmpty(options.PlexServerUrl))
            {
                Console.WriteLine(MessageConstants.EnterPlexServerUrl);
                options.PlexServerUrl = Console.ReadLine();
            }

            if (!ArgumentHelper.ValidatePlexUrl(options.PlexServerUrl))
            {
                logger.LogCritical(MessageConstants.UrlInvalid);
                return 1;
            }

            // connect to plex and get a list of libraries
            try
            {
                var client = RestClientHelper.GenerateClient(options.PlexServerUrl, options.PlexServerToken);

                var libraryRoot = await RestClientHelper.CreateAndGetRestResponse<LibraryRoot>(client, PlexApiConstants.LibrarySectionsRequestUrl, Method.Get);

                var libraries = libraryRoot?.LibraryContainer?.Libraries;

                if (libraries is null)
                {
                    logger.LogError(MessageConstants.LibrariesNoResults);
                    return 1;
                }

                // step through the libraries and get a list of the items and their folders
                foreach (var library in libraries)
                {
                    var itemRoot = await RestClientHelper.CreateAndGetRestResponse<MediaItemRoot>(client, $"{PlexApiConstants.LibrarySectionsRequestUrl}/{library.LibraryId}/{PlexApiConstants.SearchAll}", Method.Get);

                    var items = itemRoot?.MediaItemContainer?.MediaItems;

                    if (items is null)
                    {
                        logger.LogError(MessageConstants.LibraryItemsNoResults, library.LibraryName, library.LibraryType, library.LibraryId);
                        continue;
                    }

                    // step through each item in the library and get it's tvdbid and drop a .plexmatch file in it's root
                    foreach (var item in items)
                    {
                        var locationInfoRoot = await RestClientHelper.CreateAndGetRestResponse<MediaItemInfoRoot>(client, $"{PlexApiConstants.MetaDataRequestUrl}/{item.MediaItemId}", Method.Get);

                        var locationInfos = locationInfoRoot?.MediaItemInfoContainer?.MediaItemInfos;

                        if (locationInfos is null)
                        {
                            logger.LogError(MessageConstants.NoLocationInfoForItemFound, item.MediaItemTitle, item.MediaItemId);
                            continue;
                        }

                        foreach (var locationInfo in locationInfos)
                        {
                            List<IMediaPath> possibleMediaLocations = new List<IMediaPath>();

                            if (library.LibraryType == PlexApiConstants.MovieLibraryType && locationInfo.MediaInfos != null)
                            {
                                possibleMediaLocations = locationInfo.MediaInfos.SelectMany(mi => mi.MediaParts).Select(mp => (IMediaPath)mp).ToList();

                                possibleMediaLocations.ForEach(pml =>
                                {
                                    var lastForwardSlash = pml.MediaItemPath.LastIndexOf("/");
                                    var lastBackwardSlash = pml.MediaItemPath.LastIndexOf(@"\");

                                    pml.MediaItemPath = pml.MediaItemPath.Substring(0, (lastBackwardSlash > lastForwardSlash) ? lastBackwardSlash : lastForwardSlash);
                                });
                            }
                            else if ((library.LibraryType == PlexApiConstants.TVLibraryType || library.LibraryType == PlexApiConstants.MusicLibraryType) && locationInfo.MediaItemLocations != null)
                            {
                                possibleMediaLocations = locationInfo.MediaItemLocations.Select(mil => (IMediaPath)mil).ToList();
                            }
                            else
                            {
                                logger.LogWarning(MessageConstants.NoMediaFound, item.MediaItemTitle);
                                continue;
                            }

                            foreach (var location in possibleMediaLocations)
                            {
                                var mediaPath = location.MediaItemPath;

                                foreach (var rootPath in options.RootPaths)
                                {
                                    if (mediaPath.StartsWith(rootPath.PlexRootPath))
                                    {
                                        mediaPath = mediaPath.Replace(rootPath.PlexRootPath, rootPath.HostRootPath);
                                        break;
                                    }
                                }                                

                                if (Directory.Exists(mediaPath))
                                {
                                    using StreamWriter sw = new StreamWriter($"{mediaPath}/{FileConstants.PlexMatchFileName}", false);
                                    sw.WriteLine($"{MediaConstants.PlexMatchTitleHeader}{item.MediaItemTitle}");
                                    sw.WriteLine($"{MediaConstants.PlexMatchYearHeader}{item.MediaItemReleaseYear}");
                                    sw.WriteLine($"{MediaConstants.PlexMatchGuidHeader}{item.MediaItemPlexMatchGuid}");

                                    logger.LogInformation(MessageConstants.PlexMatchWritten, item.MediaItemTitle);
                                }
                                else
                                {
                                    logger.LogError(MessageConstants.FolderMissingOrInvalid, mediaPath);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(MessageConstants.ExceptionHeaderMessage);
                logger.LogError(MessageConstants.ExceptionTypeMessage, ex.GetType().ToString());
                logger.LogError(MessageConstants.ExceptionMessageMessage, ex.Message);
                if (ex.InnerException != null)
                {
                    logger.LogError(MessageConstants.ExceptionInnerExceptionTypeMessage, ex.InnerException.GetType().ToString());
                    logger.LogError(MessageConstants.ExceptionInnerExceptionMessageMessage, ex.InnerException.Message);
                }
                logger.LogError(MessageConstants.ExceptionSourceMessage, ex.Source);
                logger.LogError(MessageConstants.ExceptionStackTraceMessage, ex.StackTrace);

                return 1;
            }

            logger.LogInformation(MessageConstants.CompletedMessage);
            return 0;
        }
    }
}
