using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class Bill
{
    public int BillId { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = null!;

    public int? PersonId { get; set; }

    public virtual Person? Person { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
