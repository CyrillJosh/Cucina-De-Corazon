using System;
using System.Collections.Generic;

namespace Cucina_De_Corazon.Models;

public partial class OrderProduct
{
    public int OrderProductId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;
}
