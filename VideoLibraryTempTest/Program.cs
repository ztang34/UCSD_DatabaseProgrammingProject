using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCSD.VideoLibrary;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace VideoLibraryTempTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
            var dal = new EntLibDAL();


            Guid admin = Guid.Parse("218498B0-55B2-4EF4-A296-D2E48496457B");
            Guid user = Guid.Parse("B0281F0D-8398-4A27-BA92-828BFAA9F90E");
            Guid wrongUser = Guid.Parse("B0281F0D-8398-4A27-BA92-828BFAA9F93B");

            VideoInfo video = new VideoInfo();
            video.VideoId = 0;
            video.Director = "Zhenguan Tang";
            video.Title = "When stars falling down";
            video.Format = VideoFormat.BluRay;
            video.Year = 2019;
            video.TotalCopies = 1;

            Console.WriteLine("Start testing Search Video Library method");
            Console.ReadLine();
            //add a new video to library
            int videoID = dal.AddUpdateVideo(video, admin);
            //try search with different criterial
            var criteria = new VideoSearchCriteria()
            {
                Title = "stars falling d"
            };
            if (dal.SearchVideoLibrary(criteria).Count == 1)
            {
                Console.WriteLine("Search by title only succeeds");
            }
            else throw new Exception();

            criteria.Title = string.Empty;
            criteria.Director = "Zheng";
            if (dal.SearchVideoLibrary(criteria).Count == 1)
            {
                Console.WriteLine("Search by director name only succeeds");
            }
            else throw new Exception();

            criteria.Director = string.Empty;
            criteria.Year = 2019;
            if (dal.SearchVideoLibrary(criteria).Count == 1)
            {
                Console.WriteLine("Search by year only succeeds");
            }
            else throw new Exception();

            criteria.Director = "zheng";
            if (dal.SearchVideoLibrary(criteria).Count == 1)
            {
                Console.WriteLine("Search with director name and year succeeds");
            }
            else throw new Exception();

            criteria.Title = "stars";
            if (dal.SearchVideoLibrary(criteria).Count == 1)
            {
                Console.WriteLine("Search with all three criterias succeeds");
            }
            else throw new Exception(); 

            criteria.Title = "abc";
            if(dal.SearchVideoLibrary(criteria).Count == 0)
            {
                Console.WriteLine("Search returns empty collection when criteria cannot be met");
            }
            else throw new Exception();

            criteria = new VideoSearchCriteria();
            if (dal.SearchVideoLibrary(criteria).Count == 42)
            {
                Console.WriteLine("Search returns all records when no search criteria is provided");
            }
            else throw new Exception();

            dal.DeleteVideo(videoID, admin);
            if (dal.SearchVideoLibrary(criteria).Count == 41)
            {
                Console.WriteLine("Search returns all records when no search criteria is provided");
            }
            else throw new Exception();

            Console.WriteLine("Search Video Library method testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing Check out video method");
            Console.ReadLine();
            //videoID = dal.AddUpdateVideo(video, admin);
            dal.CheckOutVideo(1, admin);
            dal.CheckOutVideo(15, user);
            try
            {
                dal.CheckOutVideo(1, admin);
                throw new Exception();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.CheckOutVideo(15, admin);
                throw new Exception();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.CheckOutVideo(100, admin);
                throw new Exception();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.CheckOutVideo(1, wrongUser);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.CheckOutVideo(videoID, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            dal.CheckInVideo(1, admin);
            dal.CheckInVideo(15, user);
            Console.WriteLine("Checkout video method testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing Check in video method");
            Console.ReadLine();
            try
            {
                dal.CheckInVideo(100, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.CheckInVideo(1, wrongUser);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.CheckInVideo(1, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Checkin video method testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing add review method");
            Console.ReadLine();
            try
            {
                dal.AddReview(100, admin,"abc");
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddReview(1, wrongUser,"abc");
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddReview(1, admin, "");
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddReview(videoID, admin, "");
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            dal.AddReview(1, admin, "abc");
            int reviewID=dal.AddReview(1, admin, "def");
            Console.WriteLine("Add review method testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing update review method");
            Console.ReadLine();
            try
            {
                dal.UpdateReview(1000, "ghi");
                throw new Exception();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.UpdateReview(reviewID, "");
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            dal.UpdateReview(reviewID, "ghi");
            Console.WriteLine("Update review testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing add/update rating method");
            Console.ReadLine();
            try
            {
                dal.AddUpdateRating(1, wrongUser,1);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            try
            {
                dal.AddUpdateRating(100, user, 1);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddUpdateRating(1, user, 6);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddUpdateRating(1, user, 0);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddUpdateRating(1, user, 1);
                dal.AddUpdateRating(1, user, 2);
                dal.AddUpdateRating(40, user, 5);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            Console.WriteLine("Add/update rating method testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing add/update video method");
            Console.ReadLine();
            try
            {
                dal.AddUpdateVideo(video, user);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.AddUpdateVideo(video,wrongUser);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            video.VideoId = 1000;
            try
            {
                dal.AddUpdateVideo(video, admin);
                throw new Exception();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            video.VideoId = 0;
            video.Director = "";
            try
            {
                dal.AddUpdateVideo(video, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            video.Director = "Zhenguan Tang";
            video.Year = -1;
            try
            {
                dal.AddUpdateVideo(video, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            video.Year = 2019;
            video.TotalCopies = -1;
            try
            {
                dal.AddUpdateVideo(video, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            video.TotalCopies = 1;
            videoID = dal.AddUpdateVideo(video, admin);
            video.VideoId = videoID;
            video.Title = "Like a rocket shooting to the sky";
            dal.AddUpdateVideo(video, admin);
            Console.WriteLine("Add/update video method testing done");
            Console.WriteLine();

            Console.WriteLine("Start testing delete video method");
            Console.ReadLine();
            dal.CheckOutVideo(videoID, user);
            try
            {
                dal.DeleteVideo(1100, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.DeleteVideo(videoID, user);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.DeleteVideo(videoID, wrongUser);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                dal.DeleteVideo(videoID, admin);
                throw new Exception();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            dal.CheckInVideo(videoID, user);
            dal.DeleteVideo(videoID, admin);
            Console.WriteLine("Delete video method testing done");
            Console.WriteLine();


            Console.ReadLine();
        }
    }
}
