using Newtonsoft.Json;

namespace PlexMatchGenerator.RestModels
{
    public class MediaItemRoot
    {
        [JsonProperty("MediaContainer")]
        public MediaItemContainer MediaItemContainer { get; set; }
    }

    public class MediaItemContainer
    {
        [JsonProperty("Metadata")]
        public List<MediaItem> MediaItems { get; set; }
    }

    public class MediaItem
    {
        [JsonProperty("ratingKey")]
        public string MediaItemId { get; set; }
        [JsonProperty("guid")]
        public string MediaItemPlexMatchGuid { get; set; }
        [JsonProperty("title")]
        public string MediaItemTitle { get; set; }
        [JsonProperty("year")]
        public int MediaItemReleaseYear { get; set; }
    }
}