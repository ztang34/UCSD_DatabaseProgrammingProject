
namespace UCSD.VideoLibrary
{
    public class VideoSearchCriteria
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public int Year { get; set; }

        public VideoSearchCriteria()
        {
        }

        public VideoSearchCriteria(string title, int year, string director)
        {
            Title = title;
            Year = year;
            Director = director;
        }

        public bool HasOneSearchCriteria()
        {
            bool hasOneSearchCriteria = true;

            if (
            (!TitleBlank() && !DirectorBlank()) ||
            (!TitleBlank() && YearIsPostive()) ||
            (!DirectorBlank() && YearIsPostive())
            )
            {
                hasOneSearchCriteria = false;
            }
            return hasOneSearchCriteria;
        }

        public bool TitleBlank()
        {
            return ((Title == string.Empty) || (Title == null));
        }

        public bool DirectorBlank()
        {
            return ((Director == string.Empty) || (Director == null));
        }

        public bool YearIsPostive()
        {
            return (Year > 0);
        }
        public bool HasTitle { get { return !string.IsNullOrEmpty(Title); } }
        public bool HasDirector { get { return !string.IsNullOrEmpty(Director); } }
        public bool HasYear { get { return Year != -1; } }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(Title) && string.IsNullOrEmpty(Director) && Year == -1;
        }
    }
}
