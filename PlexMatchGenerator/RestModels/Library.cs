using Newtonsoft.Json;

namespace PlexMatchGenerator.RestModels
{
    public class LibraryRoot
    {
        [JsonProperty("MediaContainer")]
        public LibraryContainer LibraryContainer { get; set; }
    }

    public class LibraryContainer
    {
        [JsonProperty("Directory")]
        public List<Library> Libraries { get; set; }
    }

    public class Library
    {
        [JsonProperty("key")]
        public string LibraryId { get; set; }
        [JsonProperty("type")]
        public string LibraryType { get; set; }
        [JsonProperty("title")]
        public string LibraryName { get; set; }
    }
}