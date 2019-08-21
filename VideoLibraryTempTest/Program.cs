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
            criteria.Year = -1;

            var results = dal.SearchVideoLibrary(criteria);

            foreach(VideoSearchResult r in results)
            {
                Console.WriteLine($"{r.VideoId}, {r.Title}, {r.Year}, {r.Director}");
            }

            Console.ReadLine();
        }
    }
}
