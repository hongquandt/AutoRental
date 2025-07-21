using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class CarBrand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
