using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexMatchGenerator.RestModels
{

    public class MediaItemInfoRoot
    {
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
    }
    public class MediaItemLocation
    {
        [JsonProperty("path")]
        public string MediaItemPath { get; set; }
    }
}
