using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCSD.VideoLibrary
{
    public interface IVideoLibraryDAL
    {
        /// <summary>
        /// Search Videos table using the following rules:
        ///    a. if Title is not null or empty string, search by Title (partial match)
        ///    b. if Director is not null or empty string, search by Director (partial match)
        ///    c. if Year is positive, search by Year (full match)
        ///    d. if more than one condition applies, use AND logic
        /// </summary>
        /// <returns>Collection containing search results</returns>
        Collection<VideoSearchResult> SearchVideoLibrary(VideoSearchCriteria criteria);
        
        /// <summary>
        /// Create new row in Checkouts table if user 
        /// does not have this video checked out already
        /// </summary>
        void CheckOutVideo(int videoId, Guid userId);
        
        /// <summary>
        /// Update row in Checkouts table that corresponds to the last checkout
        /// </summary>
        void CheckInVideo(int videoId, Guid userId);
        
        /// <summary>
        /// Add new row to the Reviews table. User can create multiple reviews.
        /// </summary>
        /// <returns>Integer representing ReviewID</returns>
        int AddReview(int videoId, Guid userId, string reviewText);

        /// <summary>
        /// Update existing row in the Reviews table.
        /// </summary>
        void UpdateReview(int reviewId, string reviewText);
        
        /// <summary>
        /// Add new or update existing row in the Ratings table. 
        /// User can only have one rating for a video.
        /// </summary>
        void AddUpdateRating(int videoId, Guid userId, int rating);
        
        /// <summary>
        /// Add new or update existing row in the Videos table.
        /// Only members of the Administrator role can perform this action.
        /// </summary>
        /// <returns>Integer representing VideoID</returns>
        int AddUpdateVideo(VideoInfo video, Guid userId);

        /// <summary>
        /// Mark video as deleted. Only members of the Administrator role can perform this action.
        /// </summary>
        void DeleteVideo(int videoId, Guid userId);
    }
}
