using Microsoft.Practices.EnterpriseLibrary.Data;
using NUnit.Framework;
using System.Configuration;
using UCSD.VideoLibrary;

namespace VideoLibraryDataLayerTests
{
    [TestFixture]
    public class VideoLibraryDALFixture
    {
        private const string USER_A = "B0281F0D-8398-4A27-BA92-828BFAA9F90E";
        private const string USER_Admin = "218498B0-55B2-4EF4-A296-D2E48496457B";

        static VideoLibraryDALFixture()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
        }

        private static UCSD.VideoLibrary.IVideoLibraryDAL BuildDAL()
        {
            var connStr = ConfigurationManager.ConnectionStrings["VideoLibraryDB"].ConnectionString;

            // Reference implementation
            //return new UCSD.VideoLibrary.EFDal();
            return new UCSD.VideoLibrary.EntLibDAL();
        }

        [Test]
        public void SearchVideoLibrary1()
        {
            // Search by director name
            var dal = BuildDAL();
            var coll = dal.SearchVideoLibrary(new VideoSearchCriteria()
                {
                    Director = "Scorsese"
                });
            Assert.IsNotNull(coll, "Empty results");
            Assert.AreEqual(7, coll.Count, "Expected 7 matches for Director=Scorsese");
        }
    }
}
