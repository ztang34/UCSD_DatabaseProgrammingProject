using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoLibraryDLEFImplementation;
using System.Data.Entity;

namespace UCSD.VideoLibrary
{

    public class EFDal : IVideoLibraryDAL
    {
        public Collection<VideoSearchResult> SearchVideoLibrary(VideoSearchCriteria criteria)
        {
            var results = new Collection<VideoSearchResult>();

            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                IQueryable<Video> query;

                if (CheckEmptyCriteria(criteria))
                {
                    query = from v in context.Videos
                            where !v.IsDeleted
                            select v;
                }

                else if (criteria.HasOneSearchCriteria())
                {
                    if (criteria.HasTitle)
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Title.Contains(criteria.Title)
                                select v;
                    }
                    else if (criteria.HasDirector)
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Director.Contains(criteria.Director)
                                select v;
                    }
                    else
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Year == criteria.Year
                                select v;
                    }
                }
                else
                {
                    if (criteria.TitleBlank())
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Director.Contains(criteria.Director) && v.Year == criteria.Year
                                select v;
                    }
                    else if (criteria.DirectorBlank())
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Title.Contains(criteria.Title) && v.Year == criteria.Year
                                select v;
                    }
                    else if (!criteria.YearIsPostive())
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Title.Contains(criteria.Title) && v.Director.Contains(criteria.Director)
                                select v;
                    }
                    else
                    {
                        query = from v in context.Videos
                                where !v.IsDeleted && v.Title.Contains(criteria.Title) && v.Director.Contains(criteria.Director) && v.Year == criteria.Year
                                select v;
                    }
                }

                foreach (var video in query)
                {
                    VideoSearchResult result = new VideoSearchResult(video.VideoId, video.Title, video.Year, video.Director);
                    results.Add(result);
                }
            }
            return results;
        }

        public void CheckOutVideo(int videoId, Guid userId)
        {
            if (!ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if (!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }


            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                //check if all videos have been checked out
                if (context.Videos.First(v => v.VideoId == videoId).TotalCopies < 1)
                {
                    throw new Exception("All copies for this video have been checked out!");
                }

                //check if user has already checked out this video
                if (context.Checkouts.Count(c => c.UserId == userId && c.VideoId == videoId && c.ReturnDate == null) != 0)
                {
                    throw new Exception("You have already checked out this video!");
                }

                var video = context.Videos.Find(videoId);
                video.TotalCopies--;

                var checkout = new Checkout()
                {
                    VideoId = videoId,
                    UserId = userId,
                    CheckoutDate = DateTime.Now
                };
                context.Checkouts.Add(checkout);

                context.SaveChanges();
            }

        }

        public void CheckInVideo(int videoId, Guid userId)
        {
            if (!ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if (!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }

            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                //check if user has already checked out this video
                if (context.Checkouts.Any(c => c.UserId == userId && c.VideoId == videoId && c.ReturnDate == null))
                {
                    throw new Exception("You do not have this video!");
                }

                var checkout = context.Checkouts.OrderByDescending(c => c.CheckoutDate).First(c => c.UserId == userId && c.VideoId == videoId && c.ReturnDate == null);
                checkout.ReturnDate = DateTime.Now;
                var video = context.Videos.First(v => v.VideoId == videoId);
                video.TotalCopies++;

                context.SaveChanges();
            }
        }

        public int AddReview(int videoId, Guid userId, string reviewText)
        {
            if (!ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if (!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }

            if (String.IsNullOrEmpty(reviewText))
            {
                throw new Exception("Review text cannot be empty!");
            }

            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                var review = new Review()
                {
                    VideoId = videoId,
                    UserId = userId,
                    Review1 = reviewText
                };

                context.Reviews.Add(review);
                return review.ReviewId;
            }

        }

        public void UpdateReview(int reviewId, string reviewText)
        {
            if (!ValidateReviewID(reviewId))
            {
                throw new Exception("Cannot find the review. This could be due to wrong review ID");
            }

            if (string.IsNullOrEmpty(reviewText))
            {
                throw new Exception("Review text cannot be empty!");
            }

            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                var review = context.Reviews.First(r => r.ReviewId == reviewId);
                review.Review1 = reviewText;
                context.SaveChanges();
            }

        }

        public void AddUpdateRating(int videoId, Guid userId, int rating)
        {
            if (!ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if (!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }

            if (rating < 1 || rating > 5)
            {
                throw new Exception("Rating must be between 1 and 5");
            }

            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                var movieRating = new Rating()
                {
                    VideoId = videoId,
                    UserId = userId,
                    Rating1 = rating
                };

                context.Entry(movieRating).State = (context.Ratings.Any(r => r.VideoId == videoId && r.UserId == userId)) ? EntityState.Modified : EntityState.Added;

                context.SaveChanges();
            }
        }

        public int AddUpdateVideo(VideoInfo video, Guid userId)
        {
            int videoId = video.VideoId;
            int year = video.Year;
            int totalCopies = video.TotalCopies;

            if (videoId != 0 && !ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if (!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }

            if (!video.HasTitle || !video.HasDirector || year <= 0)
            {
                throw new Exception("Cannot update or create video record. This could be due to invalid title, director or year");
            }

            if (totalCopies < 0)
            {
                throw new Exception("Total copies cannot be negative");
            }

            if (!IsUserAdmin(userId))
            {
                throw new Exception("You cannot add or update video record because you are not an administraor");
            }

            using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
            {
                var movieVideo = new Video()
                {
                    VideoId = videoId,
                    Title = video.Title,
                    Director = video.Director,
                    Year = video.Year,
                    TotalCopies = video.TotalCopies,
                    FormatCode = TranslateVideoFormatToFormatCode(video.Format)
                };

                context.Entry(movieVideo).State = videoId == 0 ? EntityState.Added : EntityState.Modified;
                context.SaveChanges();
                return movieVideo.VideoId;
            }

        }

    
    public void DeleteVideo(int videoId, Guid userId)
    {
        if (!ValidateVideoID(videoId))
        {
            throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
        }

        if (!ValidateUserID(userId))
        {
            throw new Exception("Cannot find the user. This could be due to wrong user ID");
        }

        if (!IsUserAdmin(userId))
        {
            throw new Exception("You cannot add or update video record because you are not an administraor");
        }

        if (HasPendingCheckouts(videoId))
        {
            throw new Exception("You cannot delete video recored because there are pending check outs");
        }

        using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
        {
            var moiveVideo = context.Videos.Find(videoId);
            moiveVideo.IsDeleted = true;
            context.SaveChanges();
        }

    }

    private bool CheckEmptyCriteria(VideoSearchCriteria criteria)
    {
        bool isEmpty = false;

        if (criteria.TitleBlank() && criteria.DirectorBlank() && !criteria.YearIsPostive())
        {
            isEmpty = true;
        }

        return isEmpty;
    }

    private bool ValidateVideoID(int videoID)
    {
        bool isValidVideoID = false;

        using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
        {
            isValidVideoID = context.Videos.Any(v => v.VideoId == videoID);
        }

        return isValidVideoID;
    }

    private bool ValidateUserID(Guid userID)
    {
        bool isValidUserID = false;

        using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
        {
            isValidUserID = context.Users.Any(u => u.UserId == userID);
        }

        return isValidUserID;
    }

    private bool ValidateReviewID(int reviewID)
    {
        bool isValidReviewID = false;

        using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
        {
            if (context.Reviews.Any(r => r.ReviewId == reviewID))
            {
                isValidReviewID = true;
            }
        }

        return isValidReviewID;
    }

    private bool IsUserAdmin(Guid userID)
    {
        bool isUserAdmin = false;

        using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
        {
            if (context.Users.First(u => u.UserId == userID).UserName == "Admin")
            {
                isUserAdmin = true;
            }
        }

        return isUserAdmin;
    }


    private bool HasPendingCheckouts(int videoID)
    {
        bool hasPendingCheckouts = false;

        using (var context = new VideoLibraryDLEFImplementation.VideoLibrary())
        {
            if (context.Checkouts.Any(c => c.VideoId == videoID && c.ReturnDate == null))
            {
                hasPendingCheckouts = true;
            }
        }
        return hasPendingCheckouts;
    }

    private string TranslateVideoFormatToFormatCode(VideoFormat format)
    {
        switch (format)
        {
            case VideoFormat.BluRay:
                return "BLU";
            case VideoFormat.DVD:
                return "DVD";
            case VideoFormat.VHS:
                return "VHS";
            case VideoFormat.Unknown:
                return "Unknown";
            default:
                return "Unknown";
        }

    }
}

    
}
