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

            var criteria = new VideoSearchCriteria();
            criteria.Title = "the";
            criteria.Year = 2007;
            criteria.Director = "rob";
           

            var results = dal.SearchVideoLibrary(criteria);

            foreach(VideoSearchResult r in results)
            {
                Console.WriteLine($"{r.VideoId}, {r.Title}, {r.Year}, {r.Director}");
            }

            Guid userID = Guid.Parse("218498B0-55B2-4EF4-A296-D2E48496457B");
            VideoInfo video = new VideoInfo();
            video.VideoId = 43;
            video.Director = "Mark Zuckerberg";
            video.Title = "Dream for yesterdat";
            //video.Year = 2015;
            video.TotalCopies = 1;

            dal.DeleteVideo(22, userID);

            //Console.WriteLine(dal.AddUpdateVideo(video, userID) );

            Console.ReadLine();
        }
    }
}
