using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountName { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
