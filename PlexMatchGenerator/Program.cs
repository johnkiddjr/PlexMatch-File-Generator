// check for a paramter of the plex token, prompt if not present
using PlexMatchGenerator.Helpers;
using PlexMatchGenerator.RestModels;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

var arguments = Environment.GetCommandLineArgs().ToList();
string plexToken = string.Empty;
string plexUrl = string.Empty;

(plexUrl, plexToken) = ArgumentHelper.ProcessCommandLineArguments(arguments);

if (ArgumentHelper.CheckAndGetIfPlexUrlBlank(ref plexUrl) || ArgumentHelper.CheckAndGetIfPlexTokenBlank(ref plexToken))
{
    Console.WriteLine("Plex Server URL and Plex Token are required! Exiting...");
    return;
}

// connect to plex and get a list of libraries

var client = RestClientHelper.GenerateClient(plexUrl, plexToken);

var libraries = RestClientHelper.CreateAndGetRestResponse<LibraryRoot>(client, "library/sections", Method.Get)
    .LibraryContainer
    .Libraries;

// step through the libraries and get a list of the items and their folders
foreach (var library in libraries)
{
    var items = RestClientHelper.CreateAndGetRestResponse<MediaItemRoot>(client, $"library/sections/{library.LibraryId}/all", Method.Get)
        .MediaItemContainer
        .MediaItems;

    // step through each item in the library and get it's tvdbid and drop a .plexmatch file in it's root
    foreach (var item in items)
    {
        var locationInfos = RestClientHelper.CreateAndGetRestResponse<MediaItemInfoRoot>(client, $"library/metadata/{item.MediaItemId}", Method.Get)
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
                    using StreamWriter sw = new StreamWriter($"{location.MediaItemPath}/.plexmatch", false);
                    sw.WriteLine($"Title: {item.MediaItemTitle}");
                    sw.WriteLine($"Year: {item.MediaItemReleaseYear}");
                    sw.WriteLine($"Guid: {item.MediaItemPlexMatchGuid}");
                }
                else
                {
                    Console.WriteLine($"Folder {location.MediaItemPath} does not exist.");
                }
            }
        }
    }
}

// write to console the result of the operation
Console.WriteLine("Operation Completed");