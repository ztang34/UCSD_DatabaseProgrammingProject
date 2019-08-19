namespace VideoLibraryDLEFImplementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        public Guid ApplicationId { get; set; }

        public Guid RoleId { get; set; }

        [Required]
        [StringLength(256)]
        public string RoleName { get; set; }

        [StringLength(256)]
        public string Description { get; set; }

        public virtual Application Application { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
