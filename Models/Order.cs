using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? Email { get; set; }

    public string? CustomerAddress { get; set; }

    public string? ContactNumber { get; set; }

    public string? AlternateContactNumber { get; set; }

    public DateTime OrderDate { get; set; }

    public string? OrderStatus { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? CreatedBy { get; set; }

    public string? SpecialRequest { get; set; }

    public virtual ICollection<EventDetail> EventDetails { get; set; } = new List<EventDetail>();

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
