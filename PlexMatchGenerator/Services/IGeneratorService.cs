﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlexMatchGenerator.Abstractions;
using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.Models;
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
                    // if we are only targetting specific libraries which options.LibraryNames would be null or empty, skip the ones not in the list by matching them to the lower invarient name
                    if (options.LibraryNames != null &&
                        options.LibraryNames.Count > 0 &&
                        !options.LibraryNames.Any(ln => ln.ToLowerInvariant() == library.LibraryName.ToLowerInvariant()))
                    {
                        logger.LogInformation(MessageConstants.LibrarySkipped, library.LibraryName);
                        continue;
                    }

                    //process the library in batch format starting from 0
                    var results = await BatchProcessLibrary(client, library, options);
                    if (results.Success)
                    {
                        if (results.RecordsSkipped > 0)
                        {
                            logger.LogInformation(MessageConstants.LibraryProcessedSuccessWithSkipped, library.LibraryName, results.RecordsProcessed, results.RecordsSkipped);
                        }
                        else
                        {
                            logger.LogInformation(MessageConstants.LibraryProcessedSuccess, library.LibraryName, results.RecordsProcessed);
                        }
                    }
                    else
                    {
                        logger.LogError(MessageConstants.LibraryItemsNoResults, library.LibraryName, library.LibraryType, library.LibraryId);
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

        private async Task<ProcessingResults> BatchProcessLibrary(RestClient client, Library library, GeneratorOptions options, int startingIndex = 0, ProcessingResults carryoverResults = null)
        {
            var pagingHeaders = new Dictionary<string, string>
            {
                { PlexApiConstants.ContainerStart, startingIndex.ToString() },
                { PlexApiConstants.ContainerSize, options.ItemsPerPage.ToString() }
            };

            var itemRoot = await RestClientHelper.CreateAndGetRestResponse<MediaItemRoot>(
                client, 
                $"{PlexApiConstants.LibrarySectionsRequestUrl}/{library.LibraryId}/{PlexApiConstants.SearchAll}", 
                Method.Get,
                pagingHeaders);

            var items = itemRoot?.MediaItemContainer?.MediaItems;

            int itemsProcessed = 0;
            int itemsSkipped = 0;

            if (items == null && startingIndex == 0)
            {
                return new ProcessingResults { Success = false, RecordsProcessed = 0 };
            }
            else if (items == null)
            {
                return carryoverResults;
            }

            // step through each item in the library and drop a .plexmatch file in it's root
            foreach (var item in items)
            {
                // if we are only targetting specific shows which options.ShowNames would be null or empty, skip the ones not in the list by matching them to the lower invarient mediaitemtitle
                if (options.ShowNames != null &&
                    options.ShowNames.Count > 0 &&
                    !options.ShowNames.Any(sn => sn.ToLowerInvariant() == item.MediaItemTitle.ToLowerInvariant()))
                {
                    logger.LogInformation(MessageConstants.ShowSkipped, item.MediaItemTitle);
                    itemsSkipped++;
                    continue;
                }
                else
                {
                    itemsProcessed++;
                }

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
                            var finalWritePath = Path.Combine(mediaPath, FileConstants.PlexMatchFileName);

                            if (options.NoOverwrite && File.Exists(finalWritePath))
                            {
                                logger.LogInformation(MessageConstants.NoWriteBecauseDisabled, item.MediaItemTitle);
                                continue;
                            }

                            using StreamWriter sw = new StreamWriter(finalWritePath, false);
                            PlexMatchFileHelper.WritePlexMatchFile(sw, new PlexMatchInfo
                            {
                                FileType = PlexMatchFileType.Main,
                                MediaItemTitle = item.MediaItemTitle,
                                MediaItemReleaseYear = item.MediaItemReleaseYear,
                                MediaItemPlexMatchGuid = item.MediaItemPlexMatchGuid
                            });

                            logger.LogInformation(MessageConstants.PlexMatchWritten, item.MediaItemTitle);
                        }
                        else
                        {
                            logger.LogError(MessageConstants.FolderMissingOrInvalid, mediaPath);
                        }
                    }

                    // if per season processing is enable, or this uses non-standard sorting, process the seasons
                    if (item.MediaType == "show" && (options.EnablePerSeasonProcessing || locationInfo.ShowOrdering != ShowOrdering.Default))
                    {
                        Dictionary<string, PlexMatchInfo> seasonPaths = new Dictionary<string, PlexMatchInfo>();
                        // get the season information
                        var seasonInfo = await RestClientHelper.CreateAndGetRestResponse<MediaItemRoot>(
                            client, 
                            $"{PlexApiConstants.MetaDataRequestUrl}/{item.MediaItemId}/{PlexApiConstants.MediaItemChildren}", 
                            Method.Get);

                        // make sure there is at least 1 season
                        if(seasonInfo?.MediaItemContainer?.MediaItems?.Any() != true)
                        {
                            continue;
                        }

                        // get the season children information so we can extract the season path(s) information
                        foreach (var season in seasonInfo.MediaItemContainer.MediaItems)
                        {
                            var seasonChildrenInfo = await RestClientHelper.CreateAndGetRestResponse<MediaItemInfoRoot>(
                                client, 
                                $"{PlexApiConstants.MetaDataRequestUrl}/{season.MediaItemId}/{PlexApiConstants.MediaItemChildren}",
                                Method.Get);

                            var seasonChildren = seasonChildrenInfo?.MediaItemInfoContainer?.MediaItemInfos;

                            if(seasonChildren?.Any() != true)
                            {
                                continue;
                            }

                            var uniqueSeasonPaths = seasonChildren
                                .SelectMany(episode => episode.MediaInfos
                                    .SelectMany(info => info.MediaParts
                                        .Select(part => Path.GetDirectoryName(part.MediaItemPath))))
                                .Distinct()
                                .ToList();

                            foreach (var path in uniqueSeasonPaths)
                            {
                                if (!seasonPaths.ContainsKey(path))
                                {
                                    seasonPaths.Add(path, new PlexMatchInfo
                                    {
                                        FileType = PlexMatchFileType.Season,
                                        MediaItemTitle = item.MediaItemTitle,
                                        MediaItemReleaseYear = item.MediaItemReleaseYear,
                                        MediaItemPlexMatchGuid = season.MediaItemPlexMatchGuid, //use the season plexmatch guid
                                        SeasonNumber = season.SeasonNumber
                                    });
                                }
                            }
                        }


                        // write the plexmatch file for the season
                        foreach (var plexMatchPathAndFile in seasonPaths)
                        {
                            var mediaPath = plexMatchPathAndFile.Key;

                            foreach (var rootPath in options.RootPaths)
                            {
                                // ensure both rootpath and mediapath use the same directory separator
                                var normalizedRootPath = Path.GetFullPath(rootPath.PlexRootPath);
                                var normalizedMediaPath = Path.GetFullPath(mediaPath);
                                

                                if (normalizedMediaPath.StartsWith(normalizedRootPath))
                                {
                                    mediaPath = normalizedMediaPath.Replace(normalizedRootPath, rootPath.HostRootPath);
                                    break;
                                }
                            }

                            if (Directory.Exists(mediaPath))
                            {
                                var finalWritePath = Path.Combine(mediaPath, FileConstants.PlexMatchFileName);

                                if (options.NoOverwrite && File.Exists(finalWritePath))
                                {
                                    logger.LogInformation(MessageConstants.NoWriteBecauseDisabled, item.MediaItemTitle);
                                    continue;
                                }

                                using StreamWriter sw = new StreamWriter(finalWritePath, false);
                                PlexMatchFileHelper.WritePlexMatchFile(sw, plexMatchPathAndFile.Value);
                                logger.LogInformation(MessageConstants.PlexMatchSeasonWritten, item.MediaItemTitle, plexMatchPathAndFile.Value.SeasonNumber, plexMatchPathAndFile.Key);
                            }
                            else
                            {
                                logger.LogError(MessageConstants.FolderMissingOrInvalid, mediaPath);
                            }
                        }
                    }
                }
            }

            if (carryoverResults == null)
            {
                carryoverResults = new ProcessingResults { Success = true };
            }

            carryoverResults.RecordsProcessed += itemsProcessed;
            carryoverResults.RecordsSkipped += itemsSkipped;

            return await BatchProcessLibrary(client, library, options, startingIndex + options.ItemsPerPage, carryoverResults);
        }
    }
}
