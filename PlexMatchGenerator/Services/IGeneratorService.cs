using Microsoft.Extensions.Logging;
using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.Options;
using PlexMatchGenerator.RestModels;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexMatchGenerator.Services
{
    public interface IGeneratorService
    {
        public int Run(GeneratorOptions options);
    }

    public class GeneratorService : IGeneratorService
    {
        private readonly ILogger logger;

        public GeneratorService(ILogger<GeneratorService> logger)
        {
            this.logger = logger;
        }

        public int Run(GeneratorOptions options)
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

                var libraries = RestClientHelper.CreateAndGetRestResponse<LibraryRoot>(client, PlexApiUrlConstants.LibrarySectionsRequestUrl, Method.Get)
                    .LibraryContainer
                    .Libraries;

                // step through the libraries and get a list of the items and their folders
                foreach (var library in libraries)
                {
                    var items = RestClientHelper.CreateAndGetRestResponse<MediaItemRoot>(client, $"{PlexApiUrlConstants.LibrarySectionsRequestUrl}/{library.LibraryId}/{PlexApiUrlConstants.SearchAll}", Method.Get)
                        .MediaItemContainer
                        .MediaItems;

                    // step through each item in the library and get it's tvdbid and drop a .plexmatch file in it's root
                    foreach (var item in items)
                    {
                        var locationInfos = RestClientHelper.CreateAndGetRestResponse<MediaItemInfoRoot>(client, $"{PlexApiUrlConstants.MetaDataRequestUrl}/{item.MediaItemId}", Method.Get)
                            .MediaItemInfoContainer
                            .MediaItemInfos;

                        foreach (var locationInfo in locationInfos)
                        {
                            if (locationInfo.MediaItemLocations is null)
                            {
                                logger.LogWarning($"No media location found for: {item.MediaItemTitle}");
                                continue;
                            }

                            foreach (var location in locationInfo.MediaItemLocations)
                            {
                                if (Directory.Exists(location.MediaItemPath))
                                {
                                    using StreamWriter sw = new StreamWriter($"{location.MediaItemPath}/{MediaConstants.PlexMatchFileName}", false);
                                    sw.WriteLine($"{MediaConstants.PlexMatchTitleHeader}{item.MediaItemTitle}");
                                    sw.WriteLine($"{MediaConstants.PlexMatchYearHeader}{item.MediaItemReleaseYear}");
                                    sw.WriteLine($"{MediaConstants.PlexMatchGuidHeader}{item.MediaItemPlexMatchGuid}");

                                    logger.LogInformation($"{MessageConstants.PlexMatchWritten} {item.MediaItemTitle}");
                                }
                                else
                                {
                                    logger.LogError($"{MessageConstants.FolderMissingOrInvalid} {location.MediaItemPath}");
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
