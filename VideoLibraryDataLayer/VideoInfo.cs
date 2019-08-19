using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSD.VideoLibrary
{
    public class VideoInfo
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Director { get; set; }
        public int TotalCopies { get; set; }
        public bool IsDeleted { get; set; }
        public VideoFormat Format { get; set; }

        public VideoInfo()
        {
        }
        public VideoInfo(VideoInfo toClone)
        {
            Director = toClone.Director;
            Format = toClone.Format;
            IsDeleted = toClone.IsDeleted;
            Title = toClone.Title;
            TotalCopies = toClone.TotalCopies;
            Year = toClone.Year;
            VideoId = toClone.VideoId;
        }

        public bool HasVideoId { get { return VideoId != -1; } }
        public bool HasTitle { get { return !string.IsNullOrEmpty(Title); } }
        public bool HasYear { get { return Year != -1; } }
        public bool HasDirector { get { return !string.IsNullOrEmpty(Director); } }
        public bool HasTotalCopies { get { return TotalCopies != -1; } }
    }
}
