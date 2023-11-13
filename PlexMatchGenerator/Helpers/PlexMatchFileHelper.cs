using PlexMatchGenerator.Constants;
using PlexMatchGenerator.Models;

namespace PlexMatchGenerator.Helpers
{
    public static class PlexMatchFileHelper
    {
        public static void WritePlexMatchFile(StreamWriter writer, PlexMatchInfo info)
        {
            switch (info.FileType)
            {
                case PlexMatchFileType.Season:
                    writer.WriteLine($"{MediaConstants.PlexMatchTitleHeader}{info.MediaItemTitle}");
                    writer.WriteLine($"{MediaConstants.PlexMatchYearHeader}{info.MediaItemReleaseYear}");
                    writer.WriteLine($"{MediaConstants.PlexMatchSeasonHeader}{info.SeasonNumber}");
                    writer.WriteLine($"{MediaConstants.PlexMatchGuidHeader}{info.MediaItemPlexMatchGuid}");
                    break;
                case PlexMatchFileType.Main:
                default:
                    writer.WriteLine($"{MediaConstants.PlexMatchTitleHeader}{info.MediaItemTitle}");
                    writer.WriteLine($"{MediaConstants.PlexMatchYearHeader}{info.MediaItemReleaseYear}");
                    writer.WriteLine($"{MediaConstants.PlexMatchGuidHeader}{info.MediaItemPlexMatchGuid}");
                    break;
            }
        }
    }
}
