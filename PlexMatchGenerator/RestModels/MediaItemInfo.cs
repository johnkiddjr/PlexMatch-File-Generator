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
