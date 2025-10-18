using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public string? ProductPrice { get; set; }

    public byte[]? ProductPicture { get; set; }

    public bool IsAvailable { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
