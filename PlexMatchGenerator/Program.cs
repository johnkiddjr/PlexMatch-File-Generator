// check for a paramter of the plex token, prompt if not present
using PlexMatchGenerator.RestModels;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

var arguments = Environment.GetCommandLineArgs().ToList();
string plexToken = string.Empty;
string plexUrl = string.Empty;

if (arguments.Any())
{
    var urlArgument = arguments.Where(arg => arg == "--url" || arg == "-u").FirstOrDefault();
    var tokenArgument = arguments.Where(arg => arg == "--token" || arg == "-t").FirstOrDefault();

    if (urlArgument is null)
    {
        Console.WriteLine("No value was passed for the url... Exiting...");
        return;
    }

    if (tokenArgument is null)
    {
        Console.WriteLine("No value was passed for the token... Exiting...");
        return;
    }

    plexUrl = arguments[arguments.IndexOf(urlArgument) + 1];

    if (plexUrl.StartsWith("-") || !plexUrl.StartsWith("http"))
    {
        Console.WriteLine("Plex Url malformed. Urls must start with http or https");
        return;
    }

    plexToken = arguments[arguments.IndexOf(tokenArgument) + 1];
}

if (string.IsNullOrEmpty(plexUrl))
{
    Console.WriteLine("Please enter your Plex Server Url: ");
    plexUrl = Console.ReadLine();

    if (string.IsNullOrEmpty(plexUrl))
    {
        Console.WriteLine("No value was entered for the server url... Exiting...");
        return;
    }

    if (!plexUrl.StartsWith("http"))
    {
        Console.WriteLine("Plex Url malformed. Urls must start with http or https");
        return;
    }
}

if (string.IsNullOrEmpty(plexToken))
{
    Console.WriteLine("Please enter your Plex Token: ");
    plexToken = Console.ReadLine();

    if (plexToken == null)
    {
        Console.WriteLine("No value was entered for the token... Exiting...");
        return;
    }
}

// connect to plex and get a list of libraries
if (!plexUrl.EndsWith("/"))
{
    plexUrl += "/";
}

RestClient client = new RestClient(plexUrl);
client.UseNewtonsoftJson();
client.AddDefaultHeader("Accept", "application/json");
client.AddDefaultHeader("X-Plex-Token", plexToken);

RestRequest request = new RestRequest("library/sections", Method.Get);
var response = client.ExecuteGetAsync<LibraryRoot>(request);

response.Wait(5000);

var libraries = response.Result.Data?.LibraryContainer?.Libraries;

// step through the libraries and get a list of the items and their folders
foreach (var library in libraries)
{
    var getItemsInLibrary = new RestRequest($"library/sections/{library.LibraryId}/all", Method.Get);
    var libraryItemResponse = client.ExecuteGetAsync<MediaItemRoot>(getItemsInLibrary);
    libraryItemResponse.Wait(10000);

    var items = libraryItemResponse.Result.Data?.MediaItemContainer?.MediaItems;

    // step through each item in the library and get it's tvdbid and drop a .plexmatch file in it's root
    foreach (var item in items)
    {
        var getLocationForItem = new RestRequest($"library/metadata/{item.MediaItemId}", Method.Get);
        var itemLocationResponse = client.ExecuteGetAsync<MediaItemInfoRoot>(getLocationForItem);
        itemLocationResponse.Wait(2000);

        var locationInfos = itemLocationResponse.Result.Data?.MediaItemInfoContainer?.MediaItemInfos;

        foreach (var locationInfo in locationInfos)
        {
            foreach (var location in locationInfo.MediaItemLocations)
            {
                using System.IO.StreamWriter sw = new System.IO.StreamWriter($"{location.MediaItemPath}/.plexmatch", false);
                sw.WriteLine($"Title: {item.MediaItemTitle}");
                sw.WriteLine($"Year: {item.MediaItemReleaseYear}");
                sw.WriteLine($"Guid: {item.MediaItemPlexMatchGuid}");
            }
        }
    }
}

// write to console the result of the operation
Console.WriteLine("Operation Completed");