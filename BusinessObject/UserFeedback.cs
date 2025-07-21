using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class UserFeedback
{
    public int FeedbackId { get; set; }

    public int UserId { get; set; }

    public int? CarId { get; set; }

    public int Rating { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual Car? Car { get; set; }

    public virtual User User { get; set; } = null!;
}
