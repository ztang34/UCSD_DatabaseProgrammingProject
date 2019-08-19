namespace VideoLibraryDLEFImplementation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Checkout
    {
        public int CheckoutId { get; set; }

        public int VideoId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CheckoutDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public virtual User User { get; set; }

        public virtual Video Video { get; set; }
    }
}
