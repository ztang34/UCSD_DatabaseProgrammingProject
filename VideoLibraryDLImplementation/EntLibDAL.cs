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
            db = DatabaseFactory.CreateDatabase();
        }
        public Collection<VideoSearchResult> SearchVideoLibrary(VideoSearchCriteria criteria)
        {
            string tsql = string.Empty;
            
            if (CheckEmptyCriteria(criteria))
            {
                tsql = "SELECT * FROM dbo.Videos Where IsDeleted <> 1";           
            }

            else if (criteria.HasOneSearchCriteria())
            {
                if(criteria.HasTitle)
                {
                   tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Title Like '%{criteria.Title}%'";
                }
                else if (criteria.HasDirector)
                {
                    tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Director Like '%{criteria.Director}%'";
                }
                else
                {
                    tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Year = {criteria.Year}";
                }
            }

            else
            {
                if (criteria.TitleBlank())
                {
                    tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Director Like '%{criteria.Director}%' and Year = {criteria.Year}";
                }
                else if (criteria.DirectorBlank())
                {
                    tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Title Like '%{criteria.Title}%' and Year = {criteria.Year} ";
                }
                else if (!criteria.YearIsPostive())
                {
                    tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Director Like '%{criteria.Director}%' and Title Like '%{criteria.Title}%'";
                }
                else
                {
                    tsql = $"SELECT * FROM dbo.Videos Where IsDeleted <> 1 and Director Like '%{criteria.Director}%' and Title Like '%{criteria.Title}%' and Year = {criteria.Year}";
                }
            }

            var temp = db.ExecuteSqlStringAccessor<VideoSearchResult>(tsql);
            Collection<VideoSearchResult> result = new Collection <VideoSearchResult>(temp.ToList());

            return result;


        }

        public void CheckOutVideo(int videoId, Guid userId)
        {

            if(!ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if(!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }

            //check if all videos have been checked out
            string tsql = "SELECT TotalCopies from dbo.Videos Where VideoID = @VideoID";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            if((int)db.ExecuteScalar(cmd) < 1)
            {
                throw new Exception("All copies for this video have been checked out!");
            }

            //check if user has already checked out this video
            tsql = "SELECT count(*) from dbo.Checkouts Where UserId = @UserID and VideoID = @VideoID";
            cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userId);
            if ((int)db.ExecuteScalar(cmd) != 0)
            {
                throw new Exception("You have already checked out this video!");
            }

            tsql = "UPDATE dbo.Videos SET TotalCopies = TotalCopies -1 WHERE VideoId = @VideoID; " +
                "INSERT Into dbo.Checkouts(VideoId, UserID, CheckoutDate) Values (@VideoID, @UserID,CURRENT_TIMESTAMP) ";
            cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userId);
            db.ExecuteNonQuery(cmd);


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

            //check if user has already checked out this video
            var tsql = "SELECT count(*) from dbo.Checkouts Where UserId = @UserID and VideoID = @VideoID and ReturnDate is NULL";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userId);
            if ((int)db.ExecuteScalar(cmd) == 0)
            {
                throw new Exception("You do not have this video!");
            }


            tsql = "UPDATE dbo.Checkouts SET ReturnDate = CURRENT_TIMESTAMP WHERE CheckoutId =(SELECT TOP 1 CheckoutId FROM dbo.Checkouts WHERE UserId = @UserID and VideoId = @VideoID and ReturnDate is NULL Order by CheckoutDate Desc);" +
                "UPDATE dbo.Videos SET TotalCopies =TotalCopies + 1 WHERE VideoId = @VideoID";
            cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userId);
            db.ExecuteNonQuery(cmd);

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

            var tsql = "INSERT INTO dbo.Reviews (VideoId, UserId, Review) VALUES(@VideoID, @UserID, @ReviewText)" +
                "SELECT SCOPE_IDENTITY()";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userId);
            db.AddInParameter(cmd, "ReviewText", DbType.String, reviewText);
            return decimal.ToInt32((decimal)db.ExecuteScalar(cmd));

        }

        public void UpdateReview(int reviewId, string reviewText)
        {
            if(!ValidateReviewID(reviewId))
            {
                throw new Exception("Cannot find the review. This could be due to wrong review ID");
            }

            if(string.IsNullOrEmpty(reviewText))
            {
                throw new Exception("Review text cannot be empty!");
            }

            var tsql = "UPDATE dbo.Reviews SET Review = @ReviewText Where ReviewId = @ReviewID";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "ReviewText", DbType.String, reviewText);
            db.AddInParameter(cmd, "ReviewID", DbType.Int32, reviewId);
            db.ExecuteNonQuery(cmd);
        }

        public void AddUpdateRating(int videoId, Guid userId, int rating)
        {
            if(!ValidateVideoID(videoId))
            {
                throw new Exception("Cannot find the video. This could be due to wrong video ID or this video has been removed from library");
            }

            if (!ValidateUserID(userId))
            {
                throw new Exception("Cannot find the user. This could be due to wrong user ID");
            }

            if(rating < 1 || rating > 5)
            {
                throw new Exception("Rating must be between 1 and 5");
            }

            var tsql = "UPDATE dbo.Ratings SET Rating = @Rating WHERE VideoId = @VideoID and UserId = @UserID IF @@ROWCOUNT = 0 INSERT INTO dbo.Ratings(VideoId, UserId, Rating) VALUES(@VideoID, @UserID, @Rating)";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userId);
            db.AddInParameter(cmd, "Rating", DbType.Int32, rating);
            db.ExecuteNonQuery(cmd);

        }

        public int AddUpdateVideo(VideoInfo video, Guid userId)        
{
            int result;

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

            if(!video.HasTitle || !video.HasDirector || year <= 0)
            {
                throw new Exception("Cannot update or create video record. This could be due to invalid title, director or year");
            }

            if(totalCopies < 0)
            {
                throw new Exception("Total copies cannot be negative");
            }

            if(!IsUserInRole(userId,"Administrator"))
            {
                throw new Exception("You cannot add or update video record because you are not an administraor");
            }

            if(videoId == 0)
            {
                var tsql = "INSERT INTO dbo.Videos(Title, Director, Year, TotalCopies, FormatCode) Values(@Title, @Director, @Year, @TotalCopies, @FormatCode)" +
                    "SELECT SCOPE_IDENTITY()";
                var cmd = db.GetSqlStringCommand(tsql);
                db.AddInParameter(cmd, "Title", DbType.String, video.Title);
                db.AddInParameter(cmd, "Director", DbType.String, video.Director);
                db.AddInParameter(cmd, "Year", DbType.Int32, year);
                db.AddInParameter(cmd, "TotalCopies", DbType.Int32, totalCopies);
                db.AddInParameter(cmd, "FormatCode", DbType.String, video.Format.ToString());

                result =decimal.ToInt32((decimal)db.ExecuteScalar(cmd));
            }
            else
            {
                var tsql = "UPDATE dbo.Videos SET Title = @Title,Director = @Director, Year = @Year, TotalCopies = @TotalCopies, FormatCode = @FormatCode WHERE VideoId = @VideoID";
                var cmd = db.GetSqlStringCommand(tsql);
                db.AddInParameter(cmd, "Title", DbType.String, video.Title);
                db.AddInParameter(cmd, "Director", DbType.String, video.Director);
                db.AddInParameter(cmd, "Year", DbType.Int32, year);
                db.AddInParameter(cmd, "TotalCopies", DbType.Int32, totalCopies);
                db.AddInParameter(cmd, "FormatCode", DbType.String, video.Format.ToString());
                db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);

                db.ExecuteNonQuery(cmd);

                result = videoId;
            }

            return result;

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

            if (!IsUserInRole(userId, "Administrator"))
            {
                throw new Exception("You cannot add or update video record because you are not an administraor");
            }

            if(HasPendingCheckouts(videoId))
            {
                throw new Exception("You cannot delete video recored because there are pending check outs");
            }

            var tsql = "UPDATE dbo.Videos SET IsDeleted = 1 WHERE VideoId = @VideoID";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoId);
            db.ExecuteNonQuery(cmd);
     }

        private bool CheckEmptyCriteria(VideoSearchCriteria criteria)
        {
            bool isEmpty = false;

            if (criteria.TitleBlank() && criteria.DirectorBlank() && !criteria.YearIsPostive() )
            {
                isEmpty = true;
            }

            return isEmpty;
        }

        private bool ValidateVideoID(int videoID)
        {
            bool isValidVideoID = false;

            string tsql = "SELECT count(*) from dbo.Videos Where VideoID = @VideoID and IsDeleted<> 1";
            var cmd = db.GetSqlStringCommand(tsql);

            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoID);

            int count = (int)db.ExecuteScalar(cmd);

            if (count == 1)
            {
                isValidVideoID = true;
            }

            return isValidVideoID;
        }

        private bool ValidateUserID(Guid userID)
        {
            bool isValidUserID = false;

            string tsql = $"SELECT count(*) from dbo.Users Where UserID = @userID";
            var cmd = db.GetSqlStringCommand(tsql);

            db.AddInParameter(cmd, "UserID", DbType.Guid, userID);

            int count = (int)db.ExecuteScalar(cmd);

            if (count == 1)
            {
                isValidUserID = true;
            }

            return isValidUserID;
        }

        private bool ValidateReviewID(int reviewID)
        {
            bool isValidReviewID = false;

            string tsql = $"SELECT count(*) from dbo.Reviews Where ReviewID = @reviewID";
            var cmd = db.GetSqlStringCommand(tsql);

            db.AddInParameter(cmd, "ReviewID", DbType.Int32, reviewID);

            int count = (int)db.ExecuteScalar(cmd);

            if (count == 1)
            {
                isValidReviewID = true;
            }

            return isValidReviewID;
        }

        private bool IsUserInRole(Guid userID, string roleName)
        {
            bool isUserInRole = false;

            var tsql = "select count(*) from dbo.UsersInRoles u INNER JOIN dbo.Roles r on u.RoleId = r.RoleId WHERE r.RoleName = @roleName and u.UserId = @userID";
            var cmd = db.GetSqlStringCommand(tsql);
            db.AddInParameter(cmd, "RoleName", DbType.String, roleName);
            db.AddInParameter(cmd, "UserID", DbType.Guid, userID);

            int count = (int)db.ExecuteScalar(cmd);

            if(count == 1)
            {
                isUserInRole = true;
            }

            return isUserInRole;
        }

        private bool HasPendingCheckouts(int videoID)
        {
            bool hasPendingCheckouts = false;

            var tsql = "select count(*) from dbo.Checkouts where VideoId = @VideoID and ReturnDate is null";
            var cmd = db.GetSqlStringCommand(tsql);

            db.AddInParameter(cmd, "VideoID", DbType.Int32, videoID);

            int count = (int)db.ExecuteScalar(cmd);

            if (count == 1)
            {
                hasPendingCheckouts = true;
            }

            return hasPendingCheckouts;
        }
    }
}
