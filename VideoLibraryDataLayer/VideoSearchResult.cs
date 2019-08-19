
namespace UCSD.VideoLibrary
{
    public class VideoSearchResult
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Director { get; set; }

        public VideoSearchResult()
        {
        }

        public VideoSearchResult(int videoId, string title, int year, string director)
        {
            VideoId = videoId;
            Title = title;
            Year = year;
            Director = director;
        }
    }
}
