using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime SubmittedAt { get; set; }
}
