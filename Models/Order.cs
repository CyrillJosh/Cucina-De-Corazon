using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

    public int? NoPax { get; set; }

    public virtual EventDetail EventDetails { get; set; }
    [ValidateNever]
    public virtual IEnumerable<OrderProduct> OrderProducts { get; set; }
    [ValidateNever]
    public virtual IEnumerable<Payment> Payments { get; set; } 
}
