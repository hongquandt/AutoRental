using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public int? DiscountId { get; set; }

    public DateTime PickupDateTime { get; set; }

    public DateTime ReturnDateTime { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string BookingCode { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;
}
