using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;

namespace UCSD.VideoLibrary
{
    public class EntLibDAL : IVideoLibraryDAL
    {
        private Database db;
        public EntLibDAL ()
        {
            //DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
            db = DatabaseFactory.CreateDatabase();
        }
        public Collection<VideoSearchResult> SearchVideoLibrary(VideoSearchCriteria criteria)
        {
            IEnumerable<VideoSearchResult> temp = new Collection<VideoSearchResult>();
            
            
            if (criteria.IsEmpty())
            {
                var tsql = "SELECT * FROM dbo.Videos Where IsDeleted <> 1";
                temp = db.ExecuteSqlStringAccessor<VideoSearchResult>(tsql);
            }
            if (criteria.HasOneSearchCriteria())
            {

            }


            Collection<VideoSearchResult> result = new Collection <VideoSearchResult>(temp.ToList());

            return result;


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
