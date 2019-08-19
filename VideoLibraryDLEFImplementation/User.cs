namespace VideoLibraryDLEFImplementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public User()
        {
            Checkouts = new HashSet<Checkout>();
            Ratings = new HashSet<Rating>();
            Reviews = new HashSet<Review>();
            Roles = new HashSet<Role>();
        }

        public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        public bool IsAnonymous { get; set; }

        public DateTime LastActivityDate { get; set; }

        public virtual Application Application { get; set; }

        public virtual ICollection<Checkout> Checkouts { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
