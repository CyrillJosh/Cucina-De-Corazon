using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class EventDetail
{
    public int EventId { get; set; }

    public int OrderId { get; set; }

    public string? EventType { get; set; }

    public DateOnly EventDate { get; set; }

    public TimeOnly? EventTime { get; set; }

    public string? EventAddress { get; set; }

    public string? Theme { get; set; }

    public virtual Order Order { get; set; } = null!;
}
