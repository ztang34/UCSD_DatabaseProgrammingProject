namespace VideoLibraryDLEFImplementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Application
    {
        public Application()
        {
            Roles = new HashSet<Role>();
            Users = new HashSet<User>();
        }

        [Required]
        [StringLength(235)]
        public string ApplicationName { get; set; }

        public Guid ApplicationId { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
