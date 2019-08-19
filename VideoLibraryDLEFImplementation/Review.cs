namespace VideoLibraryDLEFImplementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Review
    {
        public int ReviewId { get; set; }

        public int VideoId { get; set; }

        public Guid UserId { get; set; }

        [Column("Review")]
        [Required]
        public string Review1 { get; set; }

        public virtual User User { get; set; }

        public virtual Video Video { get; set; }
    }
}
