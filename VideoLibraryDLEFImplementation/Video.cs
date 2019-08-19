namespace VideoLibraryDLEFImplementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Video
    {
        public Video()
        {
            Checkouts = new HashSet<Checkout>();
            Ratings = new HashSet<Rating>();
            Reviews = new HashSet<Review>();
        }

        public int VideoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int Year { get; set; }

        [Required]
        [StringLength(100)]
        public string Director { get; set; }

        [Required]
        [StringLength(10)]
        public string FormatCode { get; set; }

        public int TotalCopies { get; set; }

        public bool IsDeleted { get; set; }

        public virtual ICollection<Checkout> Checkouts { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}
