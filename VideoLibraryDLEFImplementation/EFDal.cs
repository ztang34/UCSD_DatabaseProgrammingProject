using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibraryDLEFImplementation;

namespace UCSD.VideoLibrary
{
    public class EFDal : IVideoLibraryDAL
    {
        public Collection<VideoSearchResult> SearchVideoLibrary(VideoSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public void CheckOutVideo(int videoId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public void CheckInVideo(int videoId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public int AddReview(int videoId, Guid userId, string reviewText)
        {
            throw new NotImplementedException();
        }

        public void UpdateReview(int reviewId, string reviewText)
        {
            throw new NotImplementedException();
        }

        public void AddUpdateRating(int videoId, Guid userId, int rating)
        {
            throw new NotImplementedException();
        }

        public int AddUpdateVideo(VideoInfo video, Guid userId)
        {
            throw new NotImplementedException();
        }

        public void DeleteVideo(int videoId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
