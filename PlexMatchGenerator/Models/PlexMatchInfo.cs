namespace PlexMatchGenerator.Models
{
    public class PlexMatchInfo
    {
        public PlexMatchFileType FileType { get; set; }
        public string MediaItemTitle { get; set; }
        public int MediaItemReleaseYear { get; set; }
        public string MediaItemPlexMatchGuid { get; set; }
        public int SeasonNumber { get; set; }
    }
}
