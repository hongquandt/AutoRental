using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Car
{
    public int CarId { get; set; }

    public int BrandId { get; set; }

    public string CarModel { get; set; } = null!;

    public string LicensePlate { get; set; } = null!;

    public int Seats { get; set; }

    public decimal PricePerDay { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CarBrand Brand { get; set; } = null!;

    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();

    public virtual ICollection<UserFeedback> UserFeedbacks { get; set; } = new List<UserFeedback>();
}
