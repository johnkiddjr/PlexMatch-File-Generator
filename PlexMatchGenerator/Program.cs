// check for a paramter of the plex token, prompt if not present
using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.RestModels;
using RestSharp;

var arguments = Environment.GetCommandLineArgs().ToList();
string plexToken;
string plexUrl;

(plexUrl, plexToken) = ArgumentHelper.ProcessCommandLineArguments(arguments);

if (ArgumentHelper.CheckAndGetIfPlexUrlBlank(ref plexUrl) || ArgumentHelper.CheckAndGetIfPlexTokenBlank(ref plexToken))
{
    Console.WriteLine(MessageConstants.ArgumentsMissing);
    return;
}

// connect to plex and get a list of libraries

var client = RestClientHelper.GenerateClient(plexUrl, plexToken);

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

                    Console.WriteLine($"{MessageConstants.PlexMatchWritten} {item.MediaItemTitle}");
                }
                else
                {
                    Console.WriteLine($"{MessageConstants.FolderMissingOrInvalid} {location.MediaItemPath}");
                }
            }
        }
    }
}

// write to console the result of the operation
Console.WriteLine(MessageConstants.CompletedMessage);