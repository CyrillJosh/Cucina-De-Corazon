using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? Instructions { get; set; }

    public DateTime? ReservedDate { get; set; }

    public bool IsActive { get; set; }

    public string? Address { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
}
