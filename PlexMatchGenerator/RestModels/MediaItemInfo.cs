using Newtonsoft.Json;
using PlexMatchGenerator.Abstractions;

namespace PlexMatchGenerator.RestModels
{

    public class MediaItemInfoRoot
    {
        [JsonProperty("MediaContainer")]
        public MediaItemInfoContainer MediaItemInfoContainer { get; set; }
    }

    public class MediaItemInfoContainer
    {
        [JsonProperty("Metadata")]
        public List<MediaItemInfo> MediaItemInfos { get; set; }
    }

    public class MediaItemInfo
    {
        [JsonProperty("Location")]
        public List<MediaItemLocation> MediaItemLocations { get; set; }
        [JsonProperty("Media")]
        public List<MediaInfo> MediaInfos { get; set; }
        [JsonProperty("type")]
        public string MediaType { get; set; }
        [JsonProperty("index")] // from my testing this index is always the season number... but I fear it may not always be true
        public int SeasonNumber { get; set; }
        [JsonIgnore]
        public ShowOrdering ShowOrdering
        {
            get
            {
                if (string.IsNullOrEmpty(_ordering))
                {
                    return ShowOrdering.Default;
                }
                return _ordering switch
                {
                    "absolute" => ShowOrdering.TVDBAbsolute,
                    "aired" => ShowOrdering.TVDBAired,
                    "dvd" => ShowOrdering.TVDBDVD,
                    "tmdb" => ShowOrdering.TMDBAired,
                    _ => ShowOrdering.Default
                };
            }
        }

        [JsonProperty("showOrdering")]
        private string _ordering;
    }

    public class MediaItemLocation: IMediaPath
    {
        [JsonProperty("path")]
        public string MediaItemPath { get; set; }
    }

    public class MediaInfo
    {
        [JsonProperty("Part")]
        public List<MediaInfoPart> MediaParts { get; set; }
    }

    public class MediaInfoPart: IMediaPath
    {
        [JsonProperty("file")]
        public string MediaItemPath { get; set; }
    }
}
